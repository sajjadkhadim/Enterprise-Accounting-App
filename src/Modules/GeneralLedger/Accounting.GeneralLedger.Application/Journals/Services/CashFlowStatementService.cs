
using Accounting.Domain.ChartOfAccounts.Interfaces;
using Accounting.GeneralLedger.Application.Reports;
using Accounting.GeneralLedger.Domain.Journals.Enums;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class CashFlowStatementService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IChartOfAccountsRepository _chartRepository;
    private readonly IJournalEntryRepository _journalRepository;

    public CashFlowStatementService(
        IAccountRepository accountRepository,
        IChartOfAccountsRepository chartRepository,
        IJournalEntryRepository journalRepository)
    {
        _accountRepository = accountRepository;
        _chartRepository = chartRepository;
        _journalRepository = journalRepository;
    }

    public async Task<CashFlowStatementReportDto> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid chartOfAccountsId,
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
        {
            throw new ArgumentException(
                "From date cannot be after to date.");
        }

        if (string.IsNullOrWhiteSpace(currencyCode) ||
            currencyCode.Trim().Length != 3)
        {
            throw new ArgumentException(
                "A valid currency code is required.");
        }

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

        var accounts = await _accountRepository.GetByChartAsync(
            tenantId,
            chartOfAccountsId,
            cancellationToken);

        var cashAccounts = accounts
            .Where(account => account.IsCashOrCashEquivalent)
            .ToDictionary(account => account.Id);

        if (cashAccounts.Count == 0)
        {
            throw new InvalidOperationException(
                "No cash or cash-equivalent accounts are configured in this chart of accounts.");
        }

        var journals = await _journalRepository.GetPostedJournalsAsync(
            tenantId,
            companyId,
            accountingBookId,
            fromDate,
            toDate,
            cancellationToken);

        var operatingLines = new List<CashFlowStatementLineDto>();
        var investingLines = new List<CashFlowStatementLineDto>();
        var financingLines = new List<CashFlowStatementLineDto>();

        decimal netOperatingCashFlow = 0m;
        decimal netInvestingCashFlow = 0m;
        decimal netFinancingCashFlow = 0m;

        foreach (var journal in journals)
        {
            foreach (var line in journal.Lines)
            {
                if (!cashAccounts.TryGetValue(
                    line.AccountId,
                    out var cashAccount))
                {
                    continue;
                }

                if (line.Debit.CurrencyCode != currency)
                    continue;

                // For asset accounts:
                // Debit increases cash; credit decreases cash.
                var cashMovement =
                    line.Debit.Amount - line.Credit.Amount;

                if (cashMovement == 0m ||
                    line.CashFlowActivity == CashFlowActivity.Excluded)
                {
                    continue;
                }

                var description = string.IsNullOrWhiteSpace(line.Description)
                    ? journal.Description
                    : line.Description;

                var reportLine = new CashFlowStatementLineDto(
                    line.CashFlowActivity.ToString(),
                    $"{journal.JournalNumber} - {cashAccount.Code} - {description}",
                    cashMovement);

                switch (line.CashFlowActivity)
                {
                    case CashFlowActivity.Operating:
                        operatingLines.Add(reportLine);
                        netOperatingCashFlow += cashMovement;
                        break;

                    case CashFlowActivity.Investing:
                        investingLines.Add(reportLine);
                        netInvestingCashFlow += cashMovement;
                        break;

                    case CashFlowActivity.Financing:
                        financingLines.Add(reportLine);
                        netFinancingCashFlow += cashMovement;
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Invalid cash flow activity on journal {journal.JournalNumber}.");
                }
            }
        }

        var netChangeInCash =
            netOperatingCashFlow +
            netInvestingCashFlow +
            netFinancingCashFlow;

        return new CashFlowStatementReportDto(
            companyId,
            accountingBookId,
            chartOfAccountsId,
            currency,
            fromDate,
            toDate,
            operatingLines,
            investingLines,
            financingLines,
            netOperatingCashFlow,
            netInvestingCashFlow,
            netFinancingCashFlow,
            netChangeInCash);
    }
}
