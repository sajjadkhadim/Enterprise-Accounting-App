
using Accounting.Domain.Common.Entities;

namespace Accounting.Domain.Organization.Entities;

public class Company : TenantEntity
{
    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public string LegalName { get; private set; } = string.Empty;

    public string? TaxRegistrationNumber { get; private set; }

    public string CountryCode { get; private set; } = string.Empty;

    public string BaseCurrencyCode { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Company()
    {
    }

    public Company(
        Guid tenantId,
        string name,
        string code,
        string legalName,
        string countryCode,
        string baseCurrencyCode)
        : base(tenantId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Company code is required.");

        if (string.IsNullOrWhiteSpace(legalName))
            throw new ArgumentException("Legal name is required.");

        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentException("Country code is required.");

        if (string.IsNullOrWhiteSpace(baseCurrencyCode))
            throw new ArgumentException("Base currency is required.");

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        LegalName = legalName.Trim();
        CountryCode = countryCode.Trim().ToUpperInvariant();
        BaseCurrencyCode = baseCurrencyCode.Trim().ToUpperInvariant();

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
