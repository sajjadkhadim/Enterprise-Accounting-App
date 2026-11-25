namespace Accounting.GeneralLedger.Application.Reports;

public sealed record AccountActivityReportDto(
    Guid AccountId,
    string AccountCode,
    string AccountName,
    DateOnly FromDate,
    DateOnly ToDate,
    string CurrencyCode,
    decimal OpeningDebit,
    decimal OpeningCredit,
    IReadOnlyList<AccountActivityLineDto> Transactions,
    decimal TotalDebit,
    decimal TotalCredit,
    decimal ClosingDebit,
    decimal ClosingCredit);