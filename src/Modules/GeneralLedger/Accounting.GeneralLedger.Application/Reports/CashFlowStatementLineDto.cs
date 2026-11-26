namespace Accounting.GeneralLedger.Application.Reports;

public sealed record CashFlowStatementLineDto(
    string Section,
    string Description,
    decimal Amount);