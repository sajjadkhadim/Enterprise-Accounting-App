
using Accounting.Application.Abstractions;
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class SubmitJournalForApprovalService
{
    private readonly IJournalEntryRepository _repository;
    private readonly IJournalAuditLogRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitJournalForApprovalService(
        IJournalEntryRepository repository,
        IJournalAuditLogRepository auditRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        Guid journalEntryId,
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (journalEntryId == Guid.Empty)
            throw new ArgumentException("Journal entry ID is required.");

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var journal = await _repository.GetByIdAsync(
                    journalEntryId,
                    tenantId,
                    ct);

                if (journal is null)
                    throw new KeyNotFoundException("Journal entry was not found.");

                var previousStatus = journal.Status;

                journal.SubmitForApproval();

                var auditLog = new JournalAuditLog(
                    tenantId,
                    journal.Id,
                    userId,
                    JournalAuditAction.SubmittedForApproval,
                    previousStatus,
                    journal.Status,
                    "Journal entry submitted for approval.");

                await _auditRepository.AddAsync(auditLog, ct);
                await _repository.SaveChangesAsync(ct);

                return true;
            },
            cancellationToken);
    }
}
