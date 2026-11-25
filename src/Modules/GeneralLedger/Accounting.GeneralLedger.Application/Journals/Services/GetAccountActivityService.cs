using Accounting.Domain.ChartOfAccounts.Interfaces;
using Accounting.GeneralLedger.Application.Reports;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;
using Accounting.GeneralLedger.Domain.Ledger.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class GetAccountActivityService
{
    private readonly IJournalEntryRepository _journalRepository;
    private readonly ILedgerAccountBalanceRepository _balanceRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IFiscalPeriodRepository _fiscalPeriodRepository;

    public GetAccountActivityService(
        IJournalEntryRepository journalRepository,
        ILedgerAccountBalanceRepository balanceRepository,
        IAccountRepository accountRepository,
        IFiscalPeriodRepository fiscalPeriodRepository)
    {
        _journalRepository = journalRepository;
        _balanceRepository = balanceRepository;
        _accountRepository = accountRepository;
        _fiscalPeriodRepository = fiscalPeriodRepository;
    }

    public async Task<AccountActivityReportDto> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid fiscalPeriodId,
        Guid accountId,
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty ||
            companyId == Guid.Empty ||
            accountingBookId == Guid.Empty ||
            fiscalPeriodId == Guid.Empty ||
            accountId == Guid.Empty)
        {
            throw new ArgumentException(
                "Tenant, company, book, fiscal period, and account are required.");
        }

        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code is required.");

        var currency = currencyCode.Trim().ToUpperInvariant();

        var period = await _fiscalPeriodRepository.GetByIdAsync(
            fiscalPeriodId, tenantId, cancellationToken);

        if (period is null ||
            period.CompanyId != companyId ||
            period.AccountingBookId != accountingBookId)
        {
            throw new KeyNotFoundException("Fiscal period not found.");
        }

        var account = await _accountRepository.GetByIdAsync(
            accountId, tenantId, cancellationToken);

        if (account is null)
            throw new KeyNotFoundException("Account not found.");

        var balance = await _balanceRepository.GetAsync(
            tenantId,
            accountingBookId,
            fiscalPeriodId,
            accountId,
            currency,
            cancellationToken);

        var openingDebit = balance?.OpeningDebit ?? 0m;
        var openingCredit = balance?.OpeningCredit ?? 0m;

        var journals = await _journalRepository.GetAccountActivityAsync(
            tenantId,
            companyId,
            accountingBookId,
            accountId,
            period.StartDate,
            period.EndDate,
            currency,
            cancellationToken);

        var runningNet = openingDebit - openingCredit;
        var totalDebit = 0m;
        var totalCredit = 0m;

        var transactions = new List<AccountActivityLineDto>();

        foreach (var journal in journals)
        {
            foreach (var line in journal.Lines.Where(
                line => line.AccountId == accountId &&
                        line.Debit.CurrencyCode == currency))
            {
                totalDebit += line.Debit.Amount;
                totalCredit += line.Credit.Amount;

                runningNet += line.Debit.Amount - line.Credit.Amount;

                transactions.Add(new AccountActivityLineDto(
                    journal.Id,
                    journal.JournalNumber,
                    journal.JournalDate,
                    line.Description,
                    line.Debit.Amount,
                    line.Credit.Amount,
                    Math.Max(runningNet, 0m),
                    Math.Max(-runningNet, 0m),
                    currency));
            }
        }

        var closingNet = runningNet;

        return new AccountActivityReportDto(
            accountId,
            account.Code,
            account.Name,
            period.StartDate,
            period.EndDate,
            currency,
            openingDebit,
            openingCredit,
            transactions,
            totalDebit,
            totalCredit,
            Math.Max(closingNet, 0m),
            Math.Max(-closingNet, 0m));
    }
}