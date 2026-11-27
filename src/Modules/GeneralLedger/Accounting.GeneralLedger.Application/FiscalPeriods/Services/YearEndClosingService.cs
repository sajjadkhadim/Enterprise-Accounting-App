
using Accounting.Application.Abstractions;
using Accounting.Domain.ChartOfAccounts.Enums;
using Accounting.Domain.ChartOfAccounts.Interfaces;
using Accounting.Domain.Common.Interfaces;
using Accounting.GeneralLedger.Application.Journals.Services;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Enums;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;
using Accounting.GeneralLedger.Domain.Journals.ValueObjects;
using Accounting.GeneralLedger.Domain.Ledger.Interfaces;

namespace Accounting.GeneralLedger.Application.FiscalPeriods.Services;

public sealed class YearEndClosingService
{
    private readonly IFiscalPeriodRepository _periodRepository;
    private readonly ILedgerAccountBalanceRepository _balanceRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IJournalEntryRepository _journalRepository;
    private readonly CreateJournalEntryService _createJournalService;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork;

    public YearEndClosingService(
        IFiscalPeriodRepository periodRepository,
        ILedgerAccountBalanceRepository balanceRepository,
        IAccountRepository accountRepository,
        IJournalEntryRepository journalRepository,
        CreateJournalEntryService createJournalService,
        ITenantContext tenantContext,
        IUnitOfWork unitOfWork)
    {
        _periodRepository = periodRepository;
        _balanceRepository = balanceRepository;
        _accountRepository = accountRepository;
        _journalRepository = journalRepository;
        _createJournalService = createJournalService;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<Guid>> ExecuteAsync(
        Guid fiscalPeriodId,
        Guid retainedEarningsAccountId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (fiscalPeriodId == Guid.Empty ||
            retainedEarningsAccountId == Guid.Empty ||
            userId == Guid.Empty)
        {
            throw new ArgumentException(
                "Required identifiers are missing.");
        }

        var tenantId = _tenantContext.TenantId
            ?? throw new InvalidOperationException(
                "Tenant context is missing.");

        return await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var period =
                    await _periodRepository.GetByIdForUpdateAsync(
                        fiscalPeriodId,
                        tenantId,
                        ct)
                    ?? throw new InvalidOperationException(
                        "Fiscal period was not found.");

                if (period.Status != FiscalPeriodStatus.Open)
                    throw new InvalidOperationException(
                        "Year-end closing requires an open fiscal period.");

                var hasUnpostedJournals =
                    await _journalRepository.HasUnpostedNonClosingJournalsAsync(
                        tenantId,
                        period.CompanyId,
                        period.AccountingBookId,
                        period.StartDate,
                        period.EndDate,
                        ct);

                if (hasUnpostedJournals)
                {
                    throw new InvalidOperationException(
                        "Year-end closing cannot proceed while ordinary journals remain unposted.");
                }

                var retainedEarnings =
                    await _accountRepository.GetByIdAsync(
                        retainedEarningsAccountId,
                        tenantId,
                        ct)
                    ?? throw new InvalidOperationException(
                        "Retained Earnings account was not found.");

                if (retainedEarnings.AccountType != AccountType.Equity ||
                    !retainedEarnings.IsActive ||
                    !retainedEarnings.IsPostingAllowed)
                {
                    throw new InvalidOperationException(
                        "Retained Earnings must be an active, posting-enabled Equity account.");
                }

                var balances =
                    await _balanceRepository.GetForFiscalPeriodAsync(
                        tenantId,
                        period.CompanyId,
                        period.AccountingBookId,
                        period.Id,
                        ct);

                var closingBalances =
                    new List<(Guid AccountId, string Currency, decimal Debit, decimal Credit)>();

                foreach (var balance in balances)
                {
                    var account =
                        await _accountRepository.GetByIdAsync(
                            balance.AccountId,
                            tenantId,
                            ct);

                    if (account is null)
                        throw new InvalidOperationException(
                            $"Account {balance.AccountId} was not found.");

                    if (account.AccountType != AccountType.Revenue &&
                        account.AccountType != AccountType.Expense)
                        continue;

                    if (!balance.IsValid())
                        throw new InvalidOperationException(
                            $"Invalid ledger balance for account {account.Code}.");

                    if (balance.ClosingDebit == 0 &&
                        balance.ClosingCredit == 0)
                        continue;

                    closingBalances.Add((
                        balance.AccountId,
                        balance.CurrencyCode,
                        balance.ClosingDebit,
                        balance.ClosingCredit));
                }

                if (closingBalances.Count == 0)
                    throw new InvalidOperationException(
                        "There are no Revenue or Expense balances to close.");

                var journalIds = new List<Guid>();

                foreach (var currencyGroup in closingBalances.GroupBy(
                    x => x.Currency))
                {
                    ct.ThrowIfCancellationRequested();

                    var currency = currencyGroup.Key;

                    var journalNumber =
                        $"CLOSE-{period.Id:N}-{currency}";

                    var existing =
                        await _journalRepository.GetByNumberAsync(
                            tenantId,
                            period.AccountingBookId,
                            journalNumber,
                            ct);

                    if (existing is not null)
                    {
                        if (existing.JournalType != JournalType.Closing ||
                            existing.JournalDate != period.EndDate)
                        {
                            throw new InvalidOperationException(
                                $"Journal number {journalNumber} is already used by another journal.");
                        }

                        journalIds.Add(existing.Id);
                        continue;
                    }

                    var journal = new JournalEntry(
                        tenantId,
                        period.CompanyId,
                        period.AccountingBookId,
                        journalNumber,
                        $"Year-end closing for {period.Name}",
                        period.EndDate,
                        JournalType.Closing,
                        JournalSource.System);

                    decimal totalDebits = 0;
                    decimal totalCredits = 0;

                    foreach (var balance in currencyGroup)
                    {
                        var debit = balance.Credit;
                        var credit = balance.Debit;

                        if (debit == 0 && credit == 0)
                            continue;

                        journal.AddLine(
                            balance.AccountId,
                            "Year-end income statement closing",
                            new Money(debit, currency),
                            new Money(credit, currency),
                            CashFlowActivity.Excluded);

                        totalDebits += debit;
                        totalCredits += credit;
                    }

                    var difference = totalDebits - totalCredits;

                    if (difference > 0)
                    {
                        journal.AddLine(
                            retainedEarningsAccountId,
                            "Transfer of year-end net profit",
                            new Money(0, currency),
                            new Money(difference, currency),
                            CashFlowActivity.Excluded);
                    }
                    else if (difference < 0)
                    {
                        journal.AddLine(
                            retainedEarningsAccountId,
                            "Transfer of year-end net loss",
                            new Money(Math.Abs(difference), currency),
                            new Money(0, currency),
                            CashFlowActivity.Excluded);
                    }

                    if (!journal.IsBalanced())
                        throw new InvalidOperationException(
                            $"Closing journal for {currency} is not balanced.");

                    // Reuse this transaction; do not start a nested transaction.
                    await _createJournalService.ExecuteWithinTransactionAsync(
                        journal,
                        tenantId,
                        userId,
                        ct);

                    journalIds.Add(journal.Id);
                }

                return (IReadOnlyList<Guid>)journalIds;
            },
            cancellationToken);
    }
}
