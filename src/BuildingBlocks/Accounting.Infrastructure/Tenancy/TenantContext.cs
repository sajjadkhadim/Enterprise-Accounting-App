
using Accounting.Domain.Common.Interfaces;

namespace Accounting.Infrastructure.Tenancy;

public sealed class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }

    public bool HasTenant => TenantId.HasValue;

    public void SetTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException(
                "Tenant ID cannot be empty.",
                nameof(tenantId));
        }

        if (TenantId.HasValue && TenantId.Value != tenantId)
        {
            throw new InvalidOperationException(
                "Tenant context has already been assigned.");
        }

        TenantId = tenantId;
    }
}
