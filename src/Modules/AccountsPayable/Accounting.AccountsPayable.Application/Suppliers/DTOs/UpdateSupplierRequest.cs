using System.ComponentModel.DataAnnotations;

namespace Accounting.AccountsPayable.Application.Suppliers.DTOs;

public sealed class UpdateSupplierRequest
{
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
}