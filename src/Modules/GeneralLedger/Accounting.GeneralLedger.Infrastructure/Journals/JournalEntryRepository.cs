
using Accounting.GeneralLedger.Domain.Journals.Entities;
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
}
