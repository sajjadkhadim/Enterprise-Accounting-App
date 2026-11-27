
using Accounting.GeneralLedger.Domain.FiscalPeriods.Entities;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounting.GeneralLedger.Infrastructure.FiscalPeriods;

public sealed class FiscalPeriodRepository : IFiscalPeriodRepository
{
    private readonly AppDbContext _context;

    public FiscalPeriodRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<FiscalPeriod?> GetByIdAsync(
        Guid fiscalPeriodId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<FiscalPeriod>()
            .FirstOrDefaultAsync(
                period =>
                    period.Id == fiscalPeriodId &&
                    period.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<FiscalPeriod?> GetByIdForUpdateAsync(
        Guid fiscalPeriodId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var periods = await _context.Set<FiscalPeriod>()
            .FromSqlInterpolated($"""
            SELECT *
            FROM fiscal_periods
            WHERE id = {fiscalPeriodId}
              AND tenant_id = {tenantId}
            FOR UPDATE
            """)
            .IgnoreQueryFilters()
            .AsTracking()
            .ToListAsync(cancellationToken);

        return periods.SingleOrDefault();
    }

    public async Task<FiscalPeriod?> GetForDateAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var periods = await _context.Set<FiscalPeriod>()
            .Where(period =>
                period.TenantId == tenantId &&
                period.CompanyId == companyId &&
                period.AccountingBookId == accountingBookId &&
                period.StartDate <= date &&
                period.EndDate >= date)
            .Take(2)
            .ToListAsync(cancellationToken);

        if (periods.Count > 1)
        {
            throw new InvalidOperationException(
                "Multiple fiscal periods overlap for the specified date.");
        }

        return periods.SingleOrDefault();
    }

    public async Task<FiscalPeriod?> GetForDateForUpdateAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var periods = await _context.Set<FiscalPeriod>()
            .FromSqlInterpolated($"""
            SELECT *
            FROM fiscal_periods
            WHERE tenant_id = {tenantId}
              AND company_id = {companyId}
              AND accounting_book_id = {accountingBookId}
              AND start_date <= {date}
              AND end_date >= {date}
            FOR UPDATE
            """)
            .IgnoreQueryFilters()
            .AsTracking()
            .ToListAsync(cancellationToken);

        if (periods.Count > 1)
        {
            throw new InvalidOperationException(
                "Multiple fiscal periods overlap for the specified date.");
        }

        return periods.SingleOrDefault();
    }

    public async Task<IReadOnlyList<FiscalPeriod>> GetAllAsync(
    Guid tenantId,
    Guid companyId,
    Guid accountingBookId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Set<FiscalPeriod>()
            .AsNoTracking()
            .Where(period =>
                period.TenantId == tenantId &&
                period.CompanyId == companyId &&
                period.AccountingBookId == accountingBookId)
            .OrderBy(period => period.StartDate)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> HasOverlapAsync(
    Guid tenantId,
    Guid companyId,
    Guid accountingBookId,
    DateOnly startDate,
    DateOnly endDate,
    Guid? excludeFiscalPeriodId = null,
    CancellationToken cancellationToken = default)
    {
        return _context.Set<FiscalPeriod>().AnyAsync(
            period =>
                period.TenantId == tenantId &&
                period.CompanyId == companyId &&
                period.AccountingBookId == accountingBookId &&
                period.StartDate <= endDate &&
                period.EndDate >= startDate &&
                (!excludeFiscalPeriodId.HasValue ||
                 period.Id != excludeFiscalPeriodId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        FiscalPeriod fiscalPeriod,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<FiscalPeriod>().AddAsync(
            fiscalPeriod,
            cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
