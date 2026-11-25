
using Accounting.Domain.ChartOfAccounts.Entities;
using Accounting.Domain.ChartOfAccounts.Interfaces;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _context;

    public AccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Account?> GetByIdAsync(
        Guid accountId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.Accounts.FirstOrDefaultAsync(
            account => 
                account.Id == accountId &&
                account.TenantId == tenantId,
            cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(
        Guid tenantId,
        string code,
        CancellationToken cancellationToken = default)
    {
        return _context.Accounts.AnyAsync(
            account =>
                account.TenantId == tenantId &&
                account.Code == code,
            cancellationToken);
    }

    public Task<bool> HasChildrenAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        return _context.Accounts.AnyAsync(
            account => account.ParentAccountId == accountId,
            cancellationToken);
    }

    public async Task AddAsync(
        Account account,
        CancellationToken cancellationToken = default)
    {
        await _context.Accounts.AddAsync(
            account,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
