using System.ComponentModel.DataAnnotations;

namespace Accounting.API.Contracts.GeneralLedger;

public sealed class ReverseJournalEntryRequest
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string ReversalJournalNumber { get; init; } = string.Empty;

    public DateOnly ReversalDate { get; init; }
}