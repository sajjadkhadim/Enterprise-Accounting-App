
using Accounting.AccountsPayable.Domain.Configuration.Entities;
using Accounting.AccountsPayable.Domain.Configuration.Repositories;
using Accounting.Domain.ChartOfAccounts.Entities;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounting.AccountsPayable.Infrastructure.Configuration.Repositories;

public sealed class AccountsPayableSettingsRepository
    : IAccountsPayableSettingsRepository
{
    private readonly AppDbContext _context;

    public AccountsPayableSettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<AccountsPayableSettings?> GetAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<AccountsPayableSettings>()
            .AsTracking()
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.CompanyId == companyId &&
                x.AccountingBookId == accountingBookId,
                cancellationToken);
    }

    public Task<AccountsPayableSettings?> GetByIdAsync(
        Guid id,
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<AccountsPayableSettings>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.TenantId == tenantId &&
                x.CompanyId == companyId &&
                x.AccountingBookId == accountingBookId,
                cancellationToken);
    }

    public async Task<bool> AccountsAreValidAsync(
        Guid payableControlAccountId,
        Guid? inputVatAccountId,
        Guid tenantId,
        Guid accountingBookId,
        CancellationToken cancellationToken = default)
    {
        var accountIds = new[] { payableControlAccountId, inputVatAccountId }
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();

        var validAccountIds = await (
            from account in _context.Set<Account>()
            join chart in _context.Set<ChartOfAccounts>()
                on account.ChartOfAccountsId equals chart.Id
            where account.TenantId == tenantId &&
                  account.IsActive &&
                  account.IsPostingAllowed &&
                  accountIds.Contains(account.Id) &&
                  chart.TenantId == tenantId &&
                  chart.AccountingBookId == accountingBookId
            select account.Id
        ).Distinct().ToListAsync(cancellationToken);

        return validAccountIds.Count == accountIds.Count &&
               accountIds.Count == (inputVatAccountId.HasValue ? 2 : 1);
    }

    public async Task AddAsync(
        AccountsPayableSettings settings,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<AccountsPayableSettings>()
            .AddAsync(settings, cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
