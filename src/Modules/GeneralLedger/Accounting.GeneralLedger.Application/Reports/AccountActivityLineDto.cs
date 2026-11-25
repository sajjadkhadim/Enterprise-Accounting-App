namespace Accounting.GeneralLedger.Application.Reports;

public sealed record AccountActivityLineDto(
    Guid JournalEntryId,
    string JournalNumber,
    DateOnly JournalDate,
    string Description,
    decimal Debit,
    decimal Credit,
    decimal RunningDebitBalance,
    decimal RunningCreditBalance,
    string CurrencyCode);