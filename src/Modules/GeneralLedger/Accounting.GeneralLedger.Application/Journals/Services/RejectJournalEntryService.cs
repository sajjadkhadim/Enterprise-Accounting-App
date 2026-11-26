
using Accounting.Application.Abstractions;
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class RejectJournalEntryService
{
    private readonly IJournalEntryRepository _journalRepository;
    private readonly IJournalAuditLogRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectJournalEntryService(
        IJournalEntryRepository journalRepository,
        IJournalAuditLogRepository auditRepository,
        IUnitOfWork unitOfWork)
    {
        _journalRepository = journalRepository;
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        Guid journalEntryId,
        Guid tenantId,
        Guid userId,
        string? comments = null,
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
                var journal = await _journalRepository.GetByIdAsync(
                    journalEntryId,
                    tenantId,
                    ct);

                if (journal is null)
                    throw new KeyNotFoundException("Journal entry was not found.");

                var previousStatus = journal.Status;

                journal.Reject();

                var auditLog = new JournalAuditLog(
                    tenantId,
                    journal.Id,
                    userId,
                    JournalAuditAction.Rejected,
                    previousStatus,
                    journal.Status,
                    comments ?? "Journal entry rejected.");

                await _auditRepository.AddAsync(auditLog, ct);
                await _journalRepository.SaveChangesAsync(ct);

                return true;
            },
            cancellationToken);
    }
}
