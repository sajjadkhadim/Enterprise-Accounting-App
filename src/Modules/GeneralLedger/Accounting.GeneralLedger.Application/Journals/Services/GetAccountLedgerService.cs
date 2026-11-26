
using Accounting.Domain.ChartOfAccounts.Interfaces;
using Accounting.GeneralLedger.Application.Reports;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class GetAccountLedgerService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IJournalEntryRepository _journalRepository;

    public GetAccountLedgerService(
        IAccountRepository accountRepository,
        IJournalEntryRepository journalRepository)
    {
        _accountRepository = accountRepository;
        _journalRepository = journalRepository;
    }

    public async Task<AccountLedgerReportDto> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid accountId,
        DateOnly fromDate,
        DateOnly toDate,
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty ||
            companyId == Guid.Empty ||
            accountingBookId == Guid.Empty ||
            accountId == Guid.Empty)
        {
            throw new ArgumentException("Required IDs must be provided.");
        }

        if (fromDate > toDate)
            throw new ArgumentException("From date cannot be after to date.");

        if (string.IsNullOrWhiteSpace(currencyCode) ||
            currencyCode.Trim().Length != 3)
        {
            throw new ArgumentException("A valid currency code is required.");
        }

        var currency = currencyCode.Trim().ToUpperInvariant();

        var account = await _accountRepository.GetByIdAsync(
            accountId, tenantId, cancellationToken);

        if (account is null || !account.IsActive)
            throw new KeyNotFoundException("Active account was not found.");

        var journals = await _journalRepository.GetPostedJournalsAsync(
            tenantId,
            companyId,
            accountingBookId,
            DateOnly.MinValue,
            toDate,
            cancellationToken);

        var accountTransactions = journals
            .SelectMany(journal => journal.Lines
                .Where(line =>
                    line.AccountId == accountId &&
                    line.Debit.CurrencyCode == currency)
                .Select(line => new
                {
                    Journal = journal,
                    Line = line
                }))
            .OrderBy(x => x.Journal.JournalDate)
            .ThenBy(x => x.Journal.JournalNumber)
            .ToList();

        var openingBalance = accountTransactions
            .Where(x => x.Journal.JournalDate < fromDate)
            .Sum(x => x.Line.Debit.Amount - x.Line.Credit.Amount);

        var periodTransactions = accountTransactions
            .Where(x => x.Journal.JournalDate >= fromDate)
            .ToList();

        var runningBalance = openingBalance;
        var lines = new List<AccountLedgerLineDto>();

        foreach (var item in periodTransactions)
        {
            var debit = item.Line.Debit.Amount;
            var credit = item.Line.Credit.Amount;

            runningBalance += debit - credit;

            var description = string.IsNullOrWhiteSpace(item.Line.Description)
                ? item.Journal.Description
                : item.Line.Description;

            lines.Add(new AccountLedgerLineDto(
                item.Journal.Id,
                item.Journal.JournalNumber,
                item.Journal.JournalDate,
                description,
                debit,
                credit,
                runningBalance));
        }

        var totalDebit = periodTransactions.Sum(x => x.Line.Debit.Amount);
        var totalCredit = periodTransactions.Sum(x => x.Line.Credit.Amount);

        return new AccountLedgerReportDto(
            companyId,
            accountingBookId,
            accountId,
            account.Code,
            account.Name,
            currency,
            fromDate,
            toDate,
            openingBalance,
            lines,
            totalDebit,
            totalCredit,
            runningBalance);
    }
}
