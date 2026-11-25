
using Accounting.Identity.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Identity.Authorization;

public sealed class TenantMembershipAuthorizationService : ITenantMembershipAuthorizationService
{
    private readonly IdentityDbContext _dbContext;

    public TenantMembershipAuthorizationService(
        IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasActiveMembershipAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty || tenantId == Guid.Empty)
            return false;

        return await _dbContext.TenantMemberships
            .AsNoTracking()
            .AnyAsync(
                membership =>
                    membership.UserId == userId &&
                    membership.TenantId == tenantId &&
                    membership.IsActive,
                cancellationToken);
    }
}
