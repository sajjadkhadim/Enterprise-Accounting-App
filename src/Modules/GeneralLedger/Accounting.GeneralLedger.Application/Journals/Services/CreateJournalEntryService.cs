
using Accounting.Application.Abstractions;
using Accounting.GeneralLedger.Domain.Common.Interfaces;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class CreateJournalEntryService
{
    private readonly IJournalEntryRepository _repository;
    private readonly IJournalAuditLogRepository _auditRepository;
    private readonly IJournalAccountValidator _accountValidator;
    private readonly IFiscalPeriodRepository _fiscalPeriodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateJournalEntryService(
        IJournalEntryRepository repository,
        IJournalAuditLogRepository auditRepository,
        IJournalAccountValidator accountValidator,
        IFiscalPeriodRepository fiscalPeriodRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _auditRepository = auditRepository;
        _accountValidator = accountValidator;
        _fiscalPeriodRepository = fiscalPeriodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        JournalEntry journal,
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(journal, tenantId, userId);

        await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                await ExecuteWithinTransactionAsync(
                    journal,
                    tenantId,
                    userId,
                    ct);

                return true;
            },
            cancellationToken);
    }

    // Use this method when the caller already owns the transaction.
    public async Task ExecuteWithinTransactionAsync(
        JournalEntry journal,
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(journal, tenantId, userId);

        var fiscalPeriod =
            await _fiscalPeriodRepository.GetForDateForUpdateAsync(
                tenantId,
                journal.CompanyId,
                journal.AccountingBookId,
                journal.JournalDate,
                cancellationToken);

        if (fiscalPeriod is null)
            throw new InvalidOperationException(
                "No fiscal period exists for the journal date.");

        if (!fiscalPeriod.AllowsPosting(journal.JournalDate))
            throw new InvalidOperationException(
                "Journal entries can only be created in an open fiscal period.");

        foreach (var line in journal.Lines)
        {
            var isValid =
                await _accountValidator.IsValidPostingAccountAsync(
                    tenantId,
                    journal.CompanyId,
                    journal.AccountingBookId,
                    line.AccountId,
                    cancellationToken);

            if (!isValid)
                throw new InvalidOperationException(
                    $"Account '{line.AccountId}' is not a valid posting account for this company and accounting book.");
        }

        await _repository.AddAsync(journal, cancellationToken);

        var auditLog = new JournalAuditLog(
            tenantId,
            journal.Id,
            userId,
            JournalAuditAction.Created,
            null,
            JournalStatus.Draft,
            "Journal entry created.");

        await _auditRepository.AddAsync(auditLog, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateRequest(
        JournalEntry journal,
        Guid tenantId,
        Guid userId)
    {
        ArgumentNullException.ThrowIfNull(journal);

        if (tenantId == Guid.Empty || userId == Guid.Empty)
            throw new ArgumentException(
                "Tenant ID and user ID are required.");

        if (journal.TenantId != tenantId)
            throw new InvalidOperationException(
                "The journal tenant does not match the authenticated tenant.");

        if (journal.Status != JournalStatus.Draft)
            throw new InvalidOperationException(
                "Only draft journal entries can be created.");
    }
}
