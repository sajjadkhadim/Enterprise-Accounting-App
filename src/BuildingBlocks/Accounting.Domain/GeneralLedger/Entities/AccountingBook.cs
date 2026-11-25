
using Accounting.Domain.Common.Entities;

namespace Accounting.Domain.GeneralLedger.Entities;

public class AccountingBook : TenantEntity
{
    public Guid CompanyId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public string AccountingBasis { get; private set; } = string.Empty;

    public string CurrencyCode { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private AccountingBook()
    {
    }

    public AccountingBook(
        Guid tenantId,
        Guid companyId,
        string name,
        string code,
        string accountingBasis,
        string currencyCode)
        : base(tenantId)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Book name is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Book code is required.");

        if (string.IsNullOrWhiteSpace(accountingBasis))
            throw new ArgumentException("Accounting basis is required.");

        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code is required.");

        CompanyId = companyId;
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        AccountingBasis = accountingBasis.Trim();
        CurrencyCode = currencyCode.Trim().ToUpperInvariant();

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
