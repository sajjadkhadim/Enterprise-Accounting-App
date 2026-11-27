
using Accounting.Application.Abstractions;
using Accounting.GeneralLedger.Application.Ledger.Services;
using Accounting.GeneralLedger.Domain.Common.Interfaces;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class ReverseJournalEntryService
{
    private readonly IJournalEntryRepository _journalRepository;
    private readonly IJournalAuditLogRepository _auditRepository;
    private readonly IJournalAccountValidator _accountValidator;
    private readonly IFiscalPeriodRepository _fiscalPeriodRepository;
    private readonly UpdateLedgerBalancesService _ledgerBalancesService;
    private readonly IUnitOfWork _unitOfWork;

    public ReverseJournalEntryService(
        IJournalEntryRepository journalRepository,
        IJournalAuditLogRepository auditRepository,
        IJournalAccountValidator accountValidator,
        IFiscalPeriodRepository fiscalPeriodRepository,
        UpdateLedgerBalancesService ledgerBalancesService,
        IUnitOfWork unitOfWork)
    {
        _journalRepository = journalRepository;
        _auditRepository = auditRepository;
        _accountValidator = accountValidator;
        _fiscalPeriodRepository = fiscalPeriodRepository;
        _ledgerBalancesService = ledgerBalancesService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> ExecuteAsync(
        Guid originalJournalEntryId,
        Guid tenantId,
        Guid userId,
        string reversalJournalNumber,
        DateOnly reversalDate,
        CancellationToken cancellationToken = default)
    {
        if (originalJournalEntryId == Guid.Empty)
            throw new ArgumentException("Original journal ID is required.");

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.");

        if (string.IsNullOrWhiteSpace(reversalJournalNumber))
            throw new ArgumentException("Reversal journal number is required.");

        return await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var original = await _journalRepository.GetByIdAsync(
                    originalJournalEntryId,
                    tenantId,
                    ct);

                if (original is null)
                    throw new KeyNotFoundException("Journal entry not found.");

                if (original.Status != JournalStatus.Posted)
                    throw new InvalidOperationException(
                        "Only posted journals can be reversed.");

                if (await _journalRepository.ExistsReversalAsync(
                    original.Id, tenantId, ct))
                {
                    throw new InvalidOperationException(
                        "This journal already has a reversal.");
                }

                if (original.ReversedByJournalEntryId.HasValue)
                    throw new InvalidOperationException(
                        "This journal has already been reversed.");

                var fiscalPeriod = await _fiscalPeriodRepository.GetForDateForUpdateAsync(
                    tenantId,
                    original.CompanyId,
                    original.AccountingBookId,
                    reversalDate,
                    ct);

                if (fiscalPeriod is null ||
                    !fiscalPeriod.AllowsPosting(reversalDate))
                {
                    throw new InvalidOperationException(
                        "The reversal date must belong to an open fiscal period.");
                }

                var reversal = JournalEntry.CreateReversal(
                    original,
                    reversalJournalNumber,
                    reversalDate);

                await _auditRepository.AddAsync(
                    new JournalAuditLog(
                        tenantId,
                        reversal.Id,
                        userId,
                        JournalAuditAction.Created,
                        null,
                        JournalStatus.Draft,
                        "Reversal journal created."),
                    ct);

                foreach (var line in reversal.Lines)
                {
                    var validAccount =
                        await _accountValidator.IsValidPostingAccountAsync(
                            tenantId,
                            original.CompanyId,
                            original.AccountingBookId,
                            line.AccountId,
                            ct);

                    if (!validAccount)
                        throw new InvalidOperationException(
                            $"Account {line.AccountId} is not a valid posting account.");
                }

                var previousStatus = reversal.Status;
                reversal.SubmitForApproval();

                await _auditRepository.AddAsync(
                    new JournalAuditLog(
                        tenantId,
                        reversal.Id,
                        userId,
                        JournalAuditAction.SubmittedForApproval,
                        previousStatus,
                        reversal.Status,
                        "Reversal journal submitted for approval."),
                    ct);

                previousStatus = reversal.Status;
                reversal.Approve();

                await _auditRepository.AddAsync(
                    new JournalAuditLog(
                        tenantId,
                        reversal.Id,
                        userId,
                        JournalAuditAction.Approved,
                        previousStatus,
                        reversal.Status,
                        "Reversal journal approved."),
                    ct);

                previousStatus = reversal.Status;
                reversal.Post();

                await _auditRepository.AddAsync(
                    new JournalAuditLog(
                        tenantId,
                        reversal.Id,
                        userId,
                        JournalAuditAction.Posted,
                        previousStatus,
                        reversal.Status,
                        "Reversal journal posted."),
                    ct);

                // Update ledger balances using the reversal journal lines.
                await _ledgerBalancesService.ExecuteAsync(
                    reversal,
                    fiscalPeriod.Id,
                    ct);

                var originalPreviousStatus = original.Status;
                original.MarkAsReversed(reversal.Id);

                await _auditRepository.AddAsync(
                    new JournalAuditLog(
                        tenantId,
                        original.Id,
                        userId,
                        JournalAuditAction.Reversed,
                        originalPreviousStatus,
                        original.Status,
                        $"Reversed by journal entry {reversal.Id}."),
                    ct);

                await _journalRepository.AddAsync(reversal, ct);
                await _journalRepository.SaveChangesAsync(ct);

                return reversal.Id;
            },
            cancellationToken);
    }
}
