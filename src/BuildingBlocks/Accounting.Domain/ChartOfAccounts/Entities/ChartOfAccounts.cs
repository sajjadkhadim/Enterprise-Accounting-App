
using Accounting.Domain.Common.Entities;

namespace Accounting.Domain.ChartOfAccounts.Entities;

public class ChartOfAccounts : TenantEntity
{
    public Guid AccountingBookId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private ChartOfAccounts()
    {
    }

    public ChartOfAccounts(
        Guid tenantId,
        Guid accountingBookId,
        string name,
        string code,
        string? description = null)
        : base(tenantId)
    {
        if (accountingBookId == Guid.Empty)
            throw new ArgumentException(
                "Accounting book ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Chart name is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Chart code is required.");

        AccountingBookId = accountingBookId;
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Description = description?.Trim() ?? string.Empty;

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
