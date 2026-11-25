using System.ComponentModel.DataAnnotations;
using Accounting.GeneralLedger.Domain.Journals.Enums;

namespace Accounting.API.Contracts.GeneralLedger;

public sealed class CreateJournalEntryRequest
{
    [Required]
    public Guid CompanyId { get; init; }

    [Required]
    public Guid AccountingBookId { get; init; }

    [Required, MaxLength(50)]
    public string JournalNumber { get; init; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; init; }

    public DateOnly JournalDate { get; init; }

    public JournalType JournalType { get; init; }

    public JournalSource JournalSource { get; init; }

    [Required, MinLength(2)]
    public List<CreateJournalLineRequest> Lines { get; init; } = [];
}

public sealed class CreateJournalLineRequest
{
    [Required]
    public Guid AccountId { get; init; }

    [MaxLength(500)]
    public string? Description { get; init; }

    [Range(0, 999999999999999)]
    public decimal Debit { get; init; }

    [Range(0, 999999999999999)]
    public decimal Credit { get; init; }

    [Required, StringLength(3, MinimumLength = 3)]
    public string CurrencyCode { get; init; } = string.Empty;
}