
namespace Accounting.Identity.Authorization;

public interface ITenantStatusService
{
    Task<bool> IsTenantActiveAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
