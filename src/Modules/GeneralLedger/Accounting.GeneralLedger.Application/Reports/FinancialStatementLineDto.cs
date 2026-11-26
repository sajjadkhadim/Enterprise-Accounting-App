using Accounting.Domain.ChartOfAccounts.Enums;

namespace Accounting.GeneralLedger.Application.Reports;

public sealed record FinancialStatementLineDto(
    Guid AccountId,
    string AccountCode,
    string AccountName,
    AccountType AccountType,
    AccountCategory AccountCategory,
    decimal Amount);