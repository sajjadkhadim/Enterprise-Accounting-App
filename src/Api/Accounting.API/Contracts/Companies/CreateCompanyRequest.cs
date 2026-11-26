using System.ComponentModel.DataAnnotations;

namespace Accounting.API.Contracts.Companies;

public sealed class CreateCompanyRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; init; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string LegalName { get; init; } = string.Empty;

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string CountryCode { get; init; } = string.Empty;

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string BaseCurrencyCode { get; init; } = string.Empty;
}