
using Accounting.Domain.ChartOfAccounts.Enums;
using Accounting.Domain.ChartOfAccounts.Interfaces;
using Accounting.GeneralLedger.Application.Reports;
using Accounting.GeneralLedger.Application.Reports.Enums;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;
using Accounting.GeneralLedger.Domain.Ledger.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class GetFinancialStatementService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IChartOfAccountsRepository _chartRepository;
    private readonly IJournalEntryRepository _journalRepository;
    private readonly ILedgerAccountBalanceRepository _balanceRepository;
    private readonly IFiscalPeriodRepository _fiscalPeriodRepository;

    public GetFinancialStatementService(
        IAccountRepository accountRepository,
        IChartOfAccountsRepository chartRepository,
        IJournalEntryRepository journalRepository,
        ILedgerAccountBalanceRepository balanceRepository,
        IFiscalPeriodRepository fiscalPeriodRepository)
    {
        _accountRepository = accountRepository;
        _chartRepository = chartRepository;
        _journalRepository = journalRepository;
        _balanceRepository = balanceRepository;
        _fiscalPeriodRepository = fiscalPeriodRepository;
    }

    public async Task<FinancialStatementReportDto> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid chartOfAccountsId,
        FinancialStatementType statementType,
        DateOnly fromDate,
        DateOnly toDate,
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty ||
            companyId == Guid.Empty ||
            accountingBookId == Guid.Empty ||
            chartOfAccountsId == Guid.Empty)
        {
            throw new ArgumentException(
                "Tenant, company, accounting book, and chart IDs are required.");
        }

        if (fromDate > toDate)
            throw new ArgumentException(
                "From date cannot be after to date.");

        if (string.IsNullOrWhiteSpace(currencyCode) ||
            currencyCode.Trim().Length != 3)
        {
            throw new ArgumentException("A valid currency code is required.");
        }

        if (!Enum.IsDefined(statementType))
            throw new ArgumentException("Invalid financial statement type.");

        var currency = currencyCode.Trim().ToUpperInvariant();

        var chart = await _chartRepository.GetByIdAsync(
            chartOfAccountsId,
            tenantId,
            cancellationToken);

        if (chart is null ||
            !chart.IsActive ||
            chart.AccountingBookId != accountingBookId)
        {
            throw new KeyNotFoundException(
                "Active chart of accounts was not found for this accounting book.");
        }

        DateOnly effectiveFromDate = fromDate;

        IReadOnlyList<Domain.FiscalPeriods.Entities.FiscalPeriod> periods =
            await _fiscalPeriodRepository.GetAllAsync(
                tenantId,
                companyId,
                accountingBookId,
                cancellationToken);

        var selectedPeriod = periods.FirstOrDefault(period =>
            period.StartDate <= toDate &&
            period.EndDate >= toDate);

        if (selectedPeriod is null)
        {
            throw new KeyNotFoundException(
                "No fiscal period contains the reporting end date.");
        }

        if (statementType == FinancialStatementType.BalanceSheet)
        {
            effectiveFromDate = selectedPeriod.StartDate;
        }

        var accounts = await _accountRepository.GetByChartAsync(
            tenantId,
            chartOfAccountsId,
            cancellationToken);

        var journals = await _journalRepository.GetPostedJournalsAsync(
            tenantId,
            companyId,
            accountingBookId,
            effectiveFromDate,
            toDate,
            cancellationToken);

        var openingBalances =
            await _balanceRepository.GetForFiscalPeriodAsync(
                tenantId,
                companyId,
                accountingBookId,
                selectedPeriod.Id,
                cancellationToken);

        var openingByAccount = openingBalances
            .Where(balance => balance.CurrencyCode == currency)
            .ToDictionary(
                balance => balance.AccountId,
                balance => (
                    Debit: balance.OpeningDebit,
                    Credit: balance.OpeningCredit));

        var movements = new Dictionary<Guid, (decimal Debit, decimal Credit)>();

        foreach (var journal in journals)
        {
            foreach (var line in journal.Lines)
            {
                if (line.Debit.CurrencyCode != currency)
                    continue;

                if (!movements.TryGetValue(
                    line.AccountId,
                    out var current))
                {
                    current = (0m, 0m);
                }

                movements[line.AccountId] = (
                    current.Debit + line.Debit.Amount,
                    current.Credit + line.Credit.Amount);
            }
        }

        var reportLines = new List<FinancialStatementLineDto>();

        decimal totalAssets = 0m;
        decimal totalLiabilities = 0m;
        decimal totalEquity = 0m;
        decimal totalRevenue = 0m;
        decimal totalExpenses = 0m;

        foreach (var account in accounts)
        {
            openingByAccount.TryGetValue(
                account.Id,
                out var opening);

            movements.TryGetValue(
                account.Id,
                out var movement);

            var debit = movement.Debit;
            var credit = movement.Credit;

            if (statementType == FinancialStatementType.BalanceSheet)
            {
                debit += opening.Debit;
                credit += opening.Credit;
            }

            var amount = account.AccountType is
                AccountType.Asset or AccountType.Expense
                    ? debit - credit
                    : credit - debit;

            switch (account.AccountType)
            {
                case AccountType.Asset:
                    totalAssets += amount;
                    break;

                case AccountType.Liability:
                    totalLiabilities += amount;
                    break;

                case AccountType.Equity:
                    totalEquity += amount;
                    break;

                case AccountType.Revenue:
                    totalRevenue += amount;
                    break;

                case AccountType.Expense:
                    totalExpenses += amount;
                    break;
            }

            var includeAccount =
                statementType == FinancialStatementType.BalanceSheet
                    ? account.AccountType is
                        AccountType.Asset or
                        AccountType.Liability or
                        AccountType.Equity
                    : account.AccountType is
                        AccountType.Revenue or
                        AccountType.Expense;

            if (includeAccount)
            {
                reportLines.Add(new FinancialStatementLineDto(
                    account.Id,
                    account.Code,
                    account.Name,
                    account.AccountType,
                    account.AccountCategory,
                    amount));
            }
        }

        return new FinancialStatementReportDto(
            companyId,
            accountingBookId,
            chartOfAccountsId,
            currency,
            effectiveFromDate,
            toDate,
            reportLines,
            totalAssets,
            totalLiabilities,
            totalEquity,
            totalRevenue,
            totalExpenses,
            totalRevenue - totalExpenses);
    }
}
