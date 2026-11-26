namespace Accounting.GeneralLedger.Application.Reports;

public sealed record FinancialStatementReportDto(
    Guid CompanyId,
    Guid AccountingBookId,
    Guid ChartOfAccountsId,
    string CurrencyCode,
    DateOnly FromDate,
    DateOnly ToDate,
    IReadOnlyList<FinancialStatementLineDto> Lines,
    decimal TotalAssets,
    decimal TotalLiabilities,
    decimal TotalEquity,
    decimal TotalRevenue,
    decimal TotalExpenses,
    decimal NetIncome);