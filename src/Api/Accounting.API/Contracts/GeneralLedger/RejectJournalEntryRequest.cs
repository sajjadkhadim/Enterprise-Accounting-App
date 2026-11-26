namespace Accounting.API.Contracts.GeneralLedger;

public sealed class RejectJournalEntryRequest
{
    public string? Comments { get; init; }
}