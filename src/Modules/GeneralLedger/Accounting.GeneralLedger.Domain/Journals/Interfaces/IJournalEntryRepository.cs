
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;

namespace Accounting.GeneralLedger.Domain.Journals.Interfaces;

public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(
        Guid journalEntryId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<JournalEntry?> GetByNumberAsync(
        Guid tenantId,
        Guid accountingBookId,
        string journalNumber,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        JournalEntry journal,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<JournalEntry> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        JournalStatus? status = null,
        JournalType? journalType = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default);


    Task<bool> ExistsReversalAsync(
        Guid originalJournalEntryId,
        Guid tenantId,
        CancellationToken cancellationToken = default);


    Task<IReadOnlyList<JournalEntry>> GetAccountActivityAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid accountId,
        DateOnly fromDate,
        DateOnly toDate,
        string currencyCode,
        CancellationToken cancellationToken = default);


    Task<bool> HasUnpostedJournalsAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default);

    Task<bool> HasUnpostedNonClosingJournalsAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JournalEntry>> GetPostedJournalsAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default);
}
