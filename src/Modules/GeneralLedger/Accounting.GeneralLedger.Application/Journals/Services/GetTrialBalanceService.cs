
using Accounting.Domain.ChartOfAccounts.Interfaces;
using Accounting.GeneralLedger.Application.Reports;
using Accounting.GeneralLedger.Domain.Ledger.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class GetTrialBalanceService
{
    private readonly ILedgerAccountBalanceRepository _balanceRepository;
    private readonly IAccountRepository _accountRepository;

    public GetTrialBalanceService(
        ILedgerAccountBalanceRepository balanceRepository,
        IAccountRepository accountRepository)
    {
        _balanceRepository = balanceRepository;
        _accountRepository = accountRepository;
    }

    public async Task<TrialBalanceReportDto> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid fiscalPeriodId,
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty ||
            companyId == Guid.Empty ||
            accountingBookId == Guid.Empty ||
            fiscalPeriodId == Guid.Empty)
        {
            throw new ArgumentException(
                "Tenant, company, accounting book, and fiscal period are required.");
        }

        if (string.IsNullOrWhiteSpace(currencyCode) ||
            currencyCode.Trim().Length != 3)
        {
            throw new ArgumentException("A valid currency code is required.");
        }

        var currency = currencyCode.Trim().ToUpperInvariant();

        var balances = await _balanceRepository.GetTrialBalanceAsync(
            tenantId,
            companyId,
            accountingBookId,
            fiscalPeriodId,
            currency,
            cancellationToken);

        var lines = new List<TrialBalanceLineDto>();

        foreach (var balance in balances)
        {
            var account = await _accountRepository.GetByIdAsync(
                balance.AccountId,
                tenantId,
                cancellationToken);

            if (account is null || !account.IsActive)
                continue;

            lines.Add(new TrialBalanceLineDto(
                balance.AccountId,
                account.Code,
                account.Name,
                balance.OpeningDebit,
                balance.OpeningCredit,
                balance.PeriodDebit,
                balance.PeriodCredit,
                balance.ClosingDebit,
                balance.ClosingCredit,
                balance.CurrencyCode,
                account.AccountType.ToString(),
                account.AccountCategory.ToString()));
        }

        return new TrialBalanceReportDto(
            companyId,
            accountingBookId,
            fiscalPeriodId,
            currency,
            lines,
            lines.Sum(x => x.OpeningDebit),
            lines.Sum(x => x.OpeningCredit),
            lines.Sum(x => x.PeriodDebit),
            lines.Sum(x => x.PeriodCredit),
            lines.Sum(x => x.ClosingDebit),
            lines.Sum(x => x.ClosingCredit));
    }
}
