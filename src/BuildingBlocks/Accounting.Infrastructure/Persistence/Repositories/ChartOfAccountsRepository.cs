
using COA = Accounting.Domain.ChartOfAccounts.Entities.ChartOfAccounts;
using Accounting.Domain.ChartOfAccounts.Interfaces;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Persistence.Repositories;

public sealed class ChartOfAccountsRepository : IChartOfAccountsRepository
{
    private readonly AppDbContext _context;

    public ChartOfAccountsRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<COA?> GetByIdAsync(
        Guid chartOfAccountsId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.ChartsOfAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                chart =>
                    chart.Id == chartOfAccountsId &&
                    chart.TenantId == tenantId,
                cancellationToken);
    }

    public Task<COA?> GetByAccountingBookAsync(
        Guid accountingBookId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.ChartsOfAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                chart =>
                    chart.AccountingBookId == accountingBookId &&
                    chart.TenantId == tenantId &&
                    chart.IsActive,
                cancellationToken);
    }
}
