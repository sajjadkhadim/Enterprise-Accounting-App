
using Accounting.Application.Abstractions;
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;
using Accounting.GeneralLedger.Domain.Common.Interfaces;
using Accounting.GeneralLedger.Application.Ledger.Services;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class PostJournalEntryService
{
    private readonly IJournalEntryRepository _journalRepository;
    private readonly IJournalAuditLogRepository _auditLogRepository;
    private readonly IJournalAccountValidator _accountValidator;
    private readonly IFiscalPeriodRepository _fiscalPeriodRepository;
    private readonly UpdateLedgerBalancesService _updateLedgerBalancesService;
    private readonly IUnitOfWork _unitOfWork;

    public PostJournalEntryService(
        IJournalEntryRepository journalRepository,
        IJournalAuditLogRepository auditLogRepository,
        IJournalAccountValidator accountValidator,
        IFiscalPeriodRepository fiscalPeriodRepository,
        UpdateLedgerBalancesService updateLedgerBalancesService,
        IUnitOfWork unitOfWork)
    {
        _journalRepository = journalRepository;
        _auditLogRepository = auditLogRepository;
        _accountValidator = accountValidator;
        _fiscalPeriodRepository = fiscalPeriodRepository;
        _updateLedgerBalancesService = updateLedgerBalancesService;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        Guid journalEntryId,
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(journalEntryId, tenantId, userId);

        await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                await ExecuteWithinTransactionAsync(
                    journalEntryId,
                    tenantId,
                    userId,
                    ct);

                return true;
            },
            cancellationToken);
    }

    public async Task ExecuteWithinTransactionAsync(
        Guid journalEntryId,
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(journalEntryId, tenantId, userId);

        var journal = await _journalRepository.GetByIdAsync(
            journalEntryId,
            tenantId,
            cancellationToken);

        if (journal is null)
            throw new KeyNotFoundException("Journal entry not found.");

        if (journal.Status != JournalStatus.Approved)
            throw new InvalidOperationException(
                "Only approved journals can be posted.");

        var fiscalPeriod =
            await _fiscalPeriodRepository.GetForDateForUpdateAsync(
                tenantId,
                journal.CompanyId,
                journal.AccountingBookId,
                journal.JournalDate,
                cancellationToken);

        if (fiscalPeriod is null || !fiscalPeriod.AllowsPosting(journal.JournalDate))
            throw new InvalidOperationException(
                "No open fiscal period is available for this journal date.");

        foreach (var line in journal.Lines)
        {
            var isValid = await _accountValidator.IsValidPostingAccountAsync(
                tenantId,
                journal.CompanyId,
                journal.AccountingBookId,
                line.AccountId,
                cancellationToken);

            if (!isValid)
                throw new InvalidOperationException(
                    $"Account {line.AccountId} is not a valid posting account.");
        }

        var previousStatus = journal.Status;

        journal.Post();

        await _auditLogRepository.AddAsync(
            new JournalAuditLog(
                tenantId,
                journal.Id,
                userId,
                JournalAuditAction.Posted,
                previousStatus,
                journal.Status),
            cancellationToken);

        if (journal.ReversesJournalEntryId.HasValue)
        {
            var original = await _journalRepository.GetByIdAsync(
                journal.ReversesJournalEntryId.Value,
                tenantId,
                cancellationToken);

            if (original is null)
                throw new InvalidOperationException(
                    "Original journal entry not found.");

            var originalPreviousStatus = original.Status;

            original.MarkAsReversed(journal.Id);

            await _auditLogRepository.AddAsync(
                new JournalAuditLog(
                    tenantId,
                    original.Id,
                    userId,
                    JournalAuditAction.Reversed,
                    originalPreviousStatus,
                    original.Status),
                cancellationToken);
        }

        await _updateLedgerBalancesService.ExecuteAsync(
            journal,
            fiscalPeriod.Id,
            cancellationToken);

        await _journalRepository.SaveChangesAsync(cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateRequest(
        Guid journalEntryId,
        Guid tenantId,
        Guid userId)
    {
        if (journalEntryId == Guid.Empty)
            throw new ArgumentException("Journal entry ID is required.");

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.");
    }
}
