using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class GetPagedJournalEntriesService
{
    private readonly IJournalEntryRepository _repository;

    public GetPagedJournalEntriesService(
        IJournalEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<(IReadOnlyList<JournalEntry> Items, int TotalCount)> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        JournalStatus? status = null,
        JournalType? journalType = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required.");

        if (accountingBookId == Guid.Empty)
            throw new ArgumentException("Accounting book ID is required.");

        if (pageNumber < 1)
            throw new ArgumentException("Page number must be greater than zero.");

        if (pageSize < 1 || pageSize > 200)
            throw new ArgumentException("Page size must be between 1 and 200.");

        if (fromDate.HasValue && toDate.HasValue &&
            fromDate.Value > toDate.Value)
            throw new ArgumentException("From date cannot be after to date.");

        return await _repository.GetPagedAsync(
            tenantId,
            companyId,
            accountingBookId,
            pageNumber,
            pageSize,
            searchTerm,
            status,
            journalType,
            fromDate,
            toDate,
            cancellationToken);
    }
}