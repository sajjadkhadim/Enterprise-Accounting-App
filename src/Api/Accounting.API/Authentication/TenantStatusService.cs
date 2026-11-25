
using Accounting.Identity.Authorization;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounting.API.Seeding;

public sealed class TenantStatusService : ITenantStatusService
{
    private readonly AppDbContext _appDbContext;

    public TenantStatusService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<bool> IsTenantActiveAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
            return false;

        return await _appDbContext.Tenants
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AnyAsync(
                tenant => tenant.Id == tenantId && tenant.IsActive,
                cancellationToken);
    }
}
