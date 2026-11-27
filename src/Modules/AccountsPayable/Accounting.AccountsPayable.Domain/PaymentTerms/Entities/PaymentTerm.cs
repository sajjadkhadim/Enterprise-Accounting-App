using Accounting.AccountsPayable.Domain.PaymentTerms.Enums;
using Accounting.Domain.Common.Entities;

namespace Accounting.AccountsPayable.Domain.PaymentTerms.Entities;

public sealed class PaymentTerm : TenantEntity
{
    public Guid CompanyId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int DueDays { get; private set; }
    public int? DiscountDays { get; private set; }
    public decimal? DiscountPercentage { get; private set; }
    public PaymentTermStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private PaymentTerm() { }

    public PaymentTerm(
        Guid tenantId,
        Guid companyId,
        string code,
        string name,
        int dueDays,
        int? discountDays = null,
        decimal? discountPercentage = null) : base(tenantId)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required.");

        CompanyId = companyId;
        SetCode(code);
        SetName(name);
        SetDueDays(dueDays);
        SetDiscount(discountDays, discountPercentage);

        Status = PaymentTermStatus.Active;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Update(
        string name,
        int dueDays,
        int? discountDays,
        decimal? discountPercentage)
    {
        SetName(name);
        SetDueDays(dueDays);
        SetDiscount(discountDays, discountPercentage);
    }

    public void Activate() => Status = PaymentTermStatus.Active;

    public void Deactivate() => Status = PaymentTermStatus.Inactive;

    private void SetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Payment term code is required.");

        code = code.Trim().ToUpperInvariant();

        if (code.Length > 30)
            throw new ArgumentException("Code cannot exceed 30 characters.");

        Code = code;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Payment term name is required.");

        name = name.Trim();

        if (name.Length > 100)
            throw new ArgumentException("Name cannot exceed 100 characters.");

        Name = name;
    }

    private void SetDueDays(int dueDays)
    {
        if (dueDays < 0 || dueDays > 365)
            throw new ArgumentOutOfRangeException(
                nameof(dueDays),
                "Due days must be between 0 and 365.");

        DueDays = dueDays;
    }

    private void SetDiscount(int? discountDays, decimal? discountPercentage)
    {
        if (discountDays.HasValue != discountPercentage.HasValue)
            throw new ArgumentException(
                "Discount days and percentage must both be provided.");

        if (discountDays.HasValue &&
            (discountDays.Value < 0 || discountDays.Value > DueDays))
            throw new ArgumentOutOfRangeException(nameof(discountDays));

        if (discountPercentage.HasValue &&
            (discountPercentage.Value <= 0 || discountPercentage.Value > 100))
            throw new ArgumentOutOfRangeException(nameof(discountPercentage));

        DiscountDays = discountDays;
        DiscountPercentage = discountPercentage.HasValue
            ? decimal.Round(discountPercentage.Value, 4)
            : null;
    }
}