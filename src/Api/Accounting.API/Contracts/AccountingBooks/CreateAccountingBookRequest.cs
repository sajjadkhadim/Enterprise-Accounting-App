using System.ComponentModel.DataAnnotations;

namespace Accounting.API.Contracts.AccountingBooks;

public sealed class CreateAccountingBookRequest
{
    [Required]
    public Guid CompanyId { get; init; }

    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; init; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string AccountingBasis { get; init; } = string.Empty;

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string CurrencyCode { get; init; } = string.Empty;
}