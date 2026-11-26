
namespace Accounting.GeneralLedger.Application.Reports;

public sealed record TrialBalanceReportDto(
    Guid CompanyId,
    Guid AccountingBookId,
    Guid FiscalPeriodId,
    string CurrencyCode,
    IReadOnlyList<TrialBalanceLineDto> Lines,
    decimal TotalOpeningDebit,
    decimal TotalOpeningCredit,
    decimal TotalPeriodDebit,
    decimal TotalPeriodCredit,
    decimal TotalClosingDebit,
    decimal TotalClosingCredit)
{
    public bool IsOpeningBalanced =>
        TotalOpeningDebit == TotalOpeningCredit;

    public bool IsPeriodBalanced =>
        TotalPeriodDebit == TotalPeriodCredit;

    public bool IsClosingBalanced =>
        TotalClosingDebit == TotalClosingCredit;
}
