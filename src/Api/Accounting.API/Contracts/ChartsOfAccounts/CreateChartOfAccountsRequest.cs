using System.ComponentModel.DataAnnotations;

namespace Accounting.API.Contracts.ChartsOfAccounts;

public sealed class CreateChartOfAccountsRequest
{
    [Required]
    public Guid AccountingBookId { get; init; }

    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; init; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; init; }
}