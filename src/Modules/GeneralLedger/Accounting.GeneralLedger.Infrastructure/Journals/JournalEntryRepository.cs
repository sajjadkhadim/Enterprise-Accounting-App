
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;
using Accounting.GeneralLedger.Infrastructure.Common.Exceptions;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounting.GeneralLedger.Infrastructure.Journals;

public sealed class JournalEntryRepository : IJournalEntryRepository
{
    private readonly AppDbContext _context;

    public JournalEntryRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<JournalEntry?> GetByIdAsync(
        Guid journalEntryId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<JournalEntry>()
            .Include(journal => journal.Lines)
            .FirstOrDefaultAsync(
                journal =>
                    journal.Id == journalEntryId &&
                    journal.TenantId == tenantId,
                cancellationToken);
    }

    public async Task AddAsync(
        JournalEntry journal,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<JournalEntry>()
            .AddAsync(journal, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new LedgerConcurrencyException();
        }
    }

    public async Task<(IReadOnlyList<JournalEntry> Items, int TotalCount)> GetPagedAsync(
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
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize < 1 || pageSize > 200)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        if (fromDate.HasValue && toDate.HasValue &&
            fromDate.Value > toDate.Value)
            throw new ArgumentException("From date cannot be after to date.");

        var query = _context.Set<JournalEntry>()
            .AsNoTracking()
            .Include(j => j.Lines)
            .Where(j =>
                j.TenantId == tenantId &&
                j.CompanyId == companyId &&
                j.AccountingBookId == accountingBookId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();

            query = query.Where(j =>
                j.JournalNumber.Contains(term) ||
                j.Description.Contains(term));
        }

        if (status.HasValue)
            query = query.Where(j => j.Status == status.Value);

        if (journalType.HasValue)
            query = query.Where(j => j.JournalType == journalType.Value);

        if (fromDate.HasValue)
            query = query.Where(j => j.JournalDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(j => j.JournalDate <= toDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(j => j.JournalDate)
            .ThenByDescending(j => j.JournalNumber)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsReversalAsync(
    Guid originalJournalEntryId,
    Guid tenantId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Set<JournalEntry>()
            .AnyAsync(j =>
                j.TenantId == tenantId &&
                j.ReversesJournalEntryId == originalJournalEntryId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<JournalEntry>> GetAccountActivityAsync(
    Guid tenantId,
    Guid companyId,
    Guid accountingBookId,
    Guid accountId,
    DateOnly fromDate,
    DateOnly toDate,
    string currencyCode,
    CancellationToken cancellationToken = default)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException("Account ID is required.");

        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code is required.");

        if (fromDate > toDate)
            throw new ArgumentException("From date cannot be after to date.");

        var currency = currencyCode.Trim().ToUpperInvariant();

        return await _context.Set<JournalEntry>()
            .AsNoTracking()
            .Include(j => j.Lines)
            .Where(j =>
                j.TenantId == tenantId &&
                j.CompanyId == companyId &&
                j.AccountingBookId == accountingBookId &&
                (j.Status == JournalStatus.Posted ||
                 j.Status == JournalStatus.Reversed) &&
                j.JournalDate >= fromDate &&
                j.JournalDate <= toDate &&
                j.Lines.Any(line =>
                    line.AccountId == accountId &&
                    line.Debit.CurrencyCode == currency))
            .OrderBy(j => j.JournalDate)
            .ThenBy(j => j.JournalNumber)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> HasUnpostedJournalsAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<JournalEntry>()
            .AnyAsync(journal =>
                journal.TenantId == tenantId &&
                journal.CompanyId == companyId &&
                journal.AccountingBookId == accountingBookId &&
                journal.JournalDate >= fromDate &&
                journal.JournalDate <= toDate &&
                (journal.Status == JournalStatus.Draft ||
                 journal.Status == JournalStatus.PendingApproval ||
                 journal.Status == JournalStatus.Approved),
                cancellationToken);
    }
}
