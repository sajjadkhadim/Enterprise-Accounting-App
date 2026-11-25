
namespace Accounting.Domain.Common.Entities;

public abstract class TenantEntity : BaseEntity
{
    public Guid TenantId { get; private set; }

    protected TenantEntity()
    {
    }

    protected TenantEntity(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException(
                "Tenant ID cannot be empty.",
                nameof(tenantId));
        }

        TenantId = tenantId;
    }
}
