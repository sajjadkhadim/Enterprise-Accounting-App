namespace Accounting.GeneralLedger.Application.Reports;

public sealed record TrialBalanceLineDto(
    Guid AccountId,
    string AccountCode,
    string AccountName,
    decimal OpeningDebit,
    decimal OpeningCredit,
    decimal PeriodDebit,
    decimal PeriodCredit,
    decimal ClosingDebit,
    decimal ClosingCredit,
    string CurrencyCode);