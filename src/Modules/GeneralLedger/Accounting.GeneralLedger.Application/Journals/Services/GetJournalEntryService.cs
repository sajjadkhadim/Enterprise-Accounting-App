using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class GetJournalEntryService
{
    private readonly IJournalEntryRepository _repository;

    public GetJournalEntryService(IJournalEntryRepository repository)
    {
        _repository = repository;
    }

    public Task<JournalEntry?> ExecuteAsync(
        Guid journalEntryId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        if (journalEntryId == Guid.Empty)
            throw new ArgumentException("Journal entry ID is required.");

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        return _repository.GetByIdAsync(
            journalEntryId,
            tenantId,
            cancellationToken);
    }
}