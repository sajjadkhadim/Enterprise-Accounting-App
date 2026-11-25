
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
}
