using System.ComponentModel.DataAnnotations;

namespace Accounting.AccountsPayable.Application.Suppliers.DTOs;

public sealed class CreateSupplierRequest
{
    [Required, MaxLength(50)]
    public string SupplierCode { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? LegalName { get; set; }

    [MaxLength(100)]
    public string? TaxRegistrationNumber { get; set; }

    [EmailAddress, MaxLength(254)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [StringLength(2, MinimumLength = 2)]
    public string? CountryCode { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    public string CurrencyCode { get; set; } = string.Empty;

    [Range(0, 365)]
    public int PaymentTermsDays { get; set; } = 30;

    [Range(typeof(decimal), "0", "999999999999999")]
    public decimal CreditLimit { get; set; }
}