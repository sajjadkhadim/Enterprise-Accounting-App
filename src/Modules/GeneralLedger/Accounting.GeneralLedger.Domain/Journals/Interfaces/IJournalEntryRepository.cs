
using Accounting.GeneralLedger.Domain.Journals.Entities;

namespace Accounting.GeneralLedger.Domain.Journals.Interfaces;

public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(
        Guid journalEntryId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        JournalEntry journal,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
