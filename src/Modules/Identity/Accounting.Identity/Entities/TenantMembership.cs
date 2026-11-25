
namespace Accounting.Identity.Entities;

public sealed class TenantMembership
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public Guid TenantId { get; private set; }

    public string Role { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public ApplicationUser User { get; private set; } = null!;

    private TenantMembership()
    {
    }

    public TenantMembership(
        Guid userId,
        Guid tenantId,
        string role)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.");

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role is required.");

        Id = Guid.NewGuid();
        UserId = userId;
        TenantId = tenantId;
        Role = role.Trim();
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role is required.");

        Role = role.Trim();
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
