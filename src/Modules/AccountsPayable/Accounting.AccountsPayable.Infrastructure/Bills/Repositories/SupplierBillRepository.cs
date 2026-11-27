using Accounting.AccountsPayable.Domain.Bills.Entities;
using Accounting.AccountsPayable.Domain.Bills.Repositories;
using Accounting.Domain.ChartOfAccounts.Entities;
using Accounting.Domain.GeneralLedger.Entities;
using Accounting.Domain.Organization.Entities;
using Accounting.AccountsPayable.Domain.Suppliers.Entities;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounting.AccountsPayable.Infrastructure.Bills.Repositories;

public sealed class SupplierBillRepository : ISupplierBillRepository
{
    private readonly AppDbContext _context;

    public SupplierBillRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> SupplierExistsAsync(
        Guid supplierId,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<Supplier>()
            .AnyAsync(
                x => x.Id == supplierId &&
                     x.TenantId == tenantId &&
                     x.CompanyId == companyId &&
                     x.Status == Accounting.AccountsPayable.Domain.Suppliers.Enums.SupplierStatus.Active,
                cancellationToken);
    }

    public Task<bool> AccountingBookExistsAsync(
        Guid accountingBookId,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<AccountingBook>()
            .AnyAsync(
                x => x.Id == accountingBookId &&
                     x.TenantId == tenantId &&
                     x.CompanyId == companyId,
                cancellationToken);
    }

    public async Task<bool> AccountsAreValidAsync(
        IReadOnlyCollection<Guid> accountIds,
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        CancellationToken cancellationToken = default)
    {
        if (accountIds.Count == 0)
            return false;

        var distinctIds = accountIds.Distinct().ToList();

        var accounts = await _context.Set<Account>()
            .Where(x =>
                distinctIds.Contains(x.Id) &&
                x.TenantId == tenantId &&
                x.IsActive &&
                x.IsPostingAllowed)
            .Join(
                _context.Set<ChartOfAccounts>(),
                account => account.ChartOfAccountsId,
                chart => chart.Id,
                (account, chart) => new
                {
                    Account = account,
                    Chart = chart
                })
            .Where(x =>
                x.Chart.TenantId == tenantId &&
                x.Chart.AccountingBookId == accountingBookId)
            .Select(x => x.Account.Id)
            .Distinct()
            .ToListAsync(cancellationToken);

        return accounts.Count == distinctIds.Count;
    }

    public Task<Supplier?> GetSupplierAsync(
        Guid supplierId,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<Supplier>()
            .Include(x => x.PaymentTermId)
            .FirstOrDefaultAsync(
                x => x.Id == supplierId &&
                     x.TenantId == tenantId &&
                     x.CompanyId == companyId,
                cancellationToken);
    }

    public Task<bool> BillNumberExistsAsync(
    string billNumber,
    Guid tenantId,
    Guid companyId,
    Guid accountingBookId,
    CancellationToken cancellationToken = default)
    {
        return _context.Set<SupplierBill>()
            .AnyAsync(
                x => x.BillNumber == billNumber &&
                     x.TenantId == tenantId &&
                     x.CompanyId == companyId &&
                     x.AccountingBookId == accountingBookId,
                cancellationToken);
    }

    public Task<SupplierBill?> GetByIdAsync(
        Guid billId,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<SupplierBill>()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(
                x => x.Id == billId &&
                     x.TenantId == tenantId &&
                     x.CompanyId == companyId,
                cancellationToken);
    }

    public Task AddAsync(
        SupplierBill bill,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<SupplierBill>()
            .AddAsync(bill, cancellationToken)
            .AsTask();
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}