
using Accounting.Application.Abstractions;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class ApproveJournalEntryService
{
    private readonly IJournalEntryRepository _repository;
    private readonly IJournalAuditLogRepository _auditRepository;
    private readonly IFiscalPeriodRepository _fiscalPeriodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveJournalEntryService(
        IJournalEntryRepository repository,
        IJournalAuditLogRepository auditRepository,
        IFiscalPeriodRepository fiscalPeriodRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _auditRepository = auditRepository;
        _fiscalPeriodRepository = fiscalPeriodRepository;
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

        var journal = await _repository.GetByIdAsync(
            journalEntryId,
            tenantId,
            cancellationToken);

        if (journal is null)
            throw new KeyNotFoundException(
                "Journal entry was not found.");

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
                "Journal entries can only be approved in an open fiscal period.");

        var previousStatus = journal.Status;

        journal.Approve();

        var auditLog = new JournalAuditLog(
            tenantId,
            journal.Id,
            userId,
            JournalAuditAction.Approved,
            previousStatus,
            journal.Status,
            "Journal entry approved.");

        await _auditRepository.AddAsync(auditLog, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
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
