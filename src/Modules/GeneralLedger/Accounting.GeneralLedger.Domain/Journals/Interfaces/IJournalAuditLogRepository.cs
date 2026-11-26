
using Accounting.GeneralLedger.Domain.Journals.Entities;

namespace Accounting.GeneralLedger.Domain.Journals.Interfaces;

public interface IJournalAuditLogRepository
{
    Task AddAsync(
        JournalAuditLog auditLog,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JournalAuditLog>> GetByJournalEntryAsync(
        Guid tenantId,
        Guid journalEntryId,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
