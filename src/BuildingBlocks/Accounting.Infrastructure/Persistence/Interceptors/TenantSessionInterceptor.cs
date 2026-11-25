
using Accounting.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Accounting.Infrastructure.Persistence.Interceptors;

public sealed class TenantSessionInterceptor
{
    private readonly ITenantContext _tenantContext;

    public TenantSessionInterceptor(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public async Task SetTenantAsync(
        DbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.TenantId.HasValue)
        {
            throw new InvalidOperationException(
                "A tenant must be established before accessing tenant data.");
        }

        if (dbContext.Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException(
                "Tenant session requires an active database transaction.");
        }

        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT set_config('app.current_tenant_id', {_tenantContext.TenantId.Value.ToString()}, true)",
            cancellationToken);
    }
}
