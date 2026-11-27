using Accounting.AccountsPayable.Domain.Suppliers.Enums;
using Accounting.Domain.Common.Entities;

namespace Accounting.AccountsPayable.Domain.Suppliers.Entities;

public sealed class Supplier : TenantEntity
{
    public Guid CompanyId { get; private set; }

    public string SupplierCode { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? LegalName { get; private set; }

    public string? TaxRegistrationNumber { get; private set; }

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public string? Address { get; private set; }

    public string? CountryCode { get; private set; }

    public string CurrencyCode { get; private set; } = string.Empty;

    public int PaymentTermsDays { get; private set; }

    public decimal CreditLimit { get; private set; }

    public SupplierStatus Status { get; private set; }

    public Guid? PaymentTermId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Supplier()
    {
    }

    public Supplier(
        Guid tenantId,
        Guid companyId,
        string supplierCode,
        string name,
        string currencyCode,
        int paymentTermsDays = 30,
        decimal creditLimit = 0)
        : base(tenantId)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required.");

        CompanyId = companyId;

        SetSupplierCode(supplierCode);
        SetName(name);
        SetCurrencyCode(currencyCode);

        SetPaymentTermsDays(paymentTermsDays);
        SetCreditLimit(creditLimit);

        Status = SupplierStatus.Active;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string name,
        string? legalName,
        string? taxRegistrationNumber,
        string? email,
        string? phone,
        string? address,
        string? countryCode)
    {
        SetName(name);

        LegalName = NormalizeOptional(legalName);
        TaxRegistrationNumber = NormalizeOptional(taxRegistrationNumber);
        Email = NormalizeOptional(email);
        Phone = NormalizeOptional(phone);
        Address = NormalizeOptional(address);
        CountryCode = NormalizeOptional(countryCode)?.ToUpperInvariant();
    }

    public void UpdateFinancialDetails(
        string currencyCode,
        int paymentTermsDays,
        decimal creditLimit)
    {
        SetCurrencyCode(currencyCode);
        SetPaymentTermsDays(paymentTermsDays);
        SetCreditLimit(creditLimit);
    }

    public void Activate()
    {
        Status = SupplierStatus.Active;
    }

    public void Deactivate()
    {
        Status = SupplierStatus.Inactive;
    }

    public void PutOnHold()
    {
        Status = SupplierStatus.OnHold;
    }

    private void SetSupplierCode(string supplierCode)
    {
        if (string.IsNullOrWhiteSpace(supplierCode))
            throw new ArgumentException("Supplier code is required.");

        supplierCode = supplierCode.Trim().ToUpperInvariant();

        if (supplierCode.Length > 50)
            throw new ArgumentException("Supplier code cannot exceed 50 characters.");

        SupplierCode = supplierCode;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name is required.");

        name = name.Trim();

        if (name.Length > 200)
            throw new ArgumentException("Supplier name cannot exceed 200 characters.");

        Name = name;
    }

    private void SetCurrencyCode(string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code is required.");

        currencyCode = currencyCode.Trim().ToUpperInvariant();

        if (currencyCode.Length != 3)
            throw new ArgumentException("Currency code must contain exactly 3 characters.");

        CurrencyCode = currencyCode;
    }

    private void SetPaymentTermsDays(int paymentTermsDays)
    {
        if (paymentTermsDays < 0 || paymentTermsDays > 365)
            throw new ArgumentOutOfRangeException(
                nameof(paymentTermsDays),
                "Payment terms must be between 0 and 365 days.");

        PaymentTermsDays = paymentTermsDays;
    }

    private void SetCreditLimit(decimal creditLimit)
    {
        if (creditLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(creditLimit));

        CreditLimit = decimal.Round(creditLimit, 4);
    }

    public void SetPaymentTerm(Guid? paymentTermId)
    {
        PaymentTermId = paymentTermId;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}