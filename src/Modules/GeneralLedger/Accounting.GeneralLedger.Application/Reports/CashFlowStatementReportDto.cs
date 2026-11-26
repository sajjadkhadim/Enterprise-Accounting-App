namespace Accounting.GeneralLedger.Application.Reports;

public sealed record CashFlowStatementReportDto(
    Guid CompanyId,
    Guid AccountingBookId,
    Guid ChartOfAccountsId,
    string CurrencyCode,
    DateOnly FromDate,
    DateOnly ToDate,
    IReadOnlyList<CashFlowStatementLineDto> OperatingActivities,
    IReadOnlyList<CashFlowStatementLineDto> InvestingActivities,
    IReadOnlyList<CashFlowStatementLineDto> FinancingActivities,
    decimal NetOperatingCashFlow,
    decimal NetInvestingCashFlow,
    decimal NetFinancingCashFlow,
    decimal NetChangeInCash);