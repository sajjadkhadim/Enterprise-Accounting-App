
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounting.GeneralLedger.Infrastructure.Persistence.Repositories;

public sealed class JournalAuditLogRepository : IJournalAuditLogRepository
{
    private readonly AppDbContext _context;

    public JournalAuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        JournalAuditLog auditLog,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<JournalAuditLog>()
            .AddAsync(auditLog, cancellationToken);
    }

    public async Task<IReadOnlyList<JournalAuditLog>> GetByJournalEntryAsync(
        Guid tenantId,
        Guid journalEntryId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<JournalAuditLog>()
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                x.JournalEntryId == journalEntryId)
            .OrderBy(x => x.OccurredAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
