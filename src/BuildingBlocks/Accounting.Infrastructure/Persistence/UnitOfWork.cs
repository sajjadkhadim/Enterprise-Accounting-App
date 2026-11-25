
using Accounting.Application.Abstractions;
using Accounting.Domain.Common.Interfaces;
using Accounting.Infrastructure.Persistence.Context;
using Accounting.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly TenantSessionInterceptor _tenantSession;
    private readonly ITenantContext _tenantContext;

    public UnitOfWork(
        AppDbContext context,
        TenantSessionInterceptor tenantSession,
        ITenantContext tenantContext)
    {
        _context = context;
        _tenantSession = tenantSession;
        _tenantContext = tenantContext;
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.TenantId.HasValue)
        {
            throw new InvalidOperationException(
                "A tenant must be established before accessing tenant data.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            await _tenantSession.SetTenantAsync(
                _context,
                cancellationToken);

            var result = await operation(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
