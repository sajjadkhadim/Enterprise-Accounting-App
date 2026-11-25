using Accounting.Domain.Common.Entities;

namespace Accounting.Domain.Tenancy.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Tenant()
    {
    }

    public Tenant(string name, string code)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Tenant name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Tenant code is required.", nameof(code));

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}