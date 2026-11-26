
namespace Accounting.GeneralLedger.Application.Reports;

public sealed record AccountLedgerReportDto(
    Guid CompanyId,
    Guid AccountingBookId,
    Guid AccountId,
    string AccountCode,
    string AccountName,
    string CurrencyCode,
    DateOnly FromDate,
    DateOnly ToDate,
    decimal OpeningBalance,
    IReadOnlyList<AccountLedgerLineDto> Transactions,
    decimal TotalDebit,
    decimal TotalCredit,
    decimal ClosingBalance);
