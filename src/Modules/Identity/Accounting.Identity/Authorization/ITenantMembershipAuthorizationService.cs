
namespace Accounting.Identity.Authorization;

public interface ITenantMembershipAuthorizationService
{
    Task<bool> HasActiveMembershipAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
