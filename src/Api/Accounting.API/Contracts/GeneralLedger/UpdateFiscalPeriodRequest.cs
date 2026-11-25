using System.ComponentModel.DataAnnotations;

namespace Accounting.API.Contracts.GeneralLedger;

public sealed class UpdateFiscalPeriodRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; init; }

    [Required]
    public DateOnly EndDate { get; init; }
}