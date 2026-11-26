
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Ledger.Entities;
using Accounting.GeneralLedger.Domain.Ledger.Interfaces;

namespace Accounting.GeneralLedger.Application.Ledger.Services;

public sealed class UpdateLedgerBalancesService
{
    private readonly ILedgerAccountBalanceRepository _balanceRepository;

    public UpdateLedgerBalancesService(
        ILedgerAccountBalanceRepository balanceRepository)
    {
        _balanceRepository = balanceRepository;
    }

    public async Task ExecuteAsync(
        JournalEntry journal,
        Guid fiscalPeriodId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(journal);

        if (journal.TenantId == Guid.Empty)
            throw new InvalidOperationException("Journal tenant is required.");

        if (fiscalPeriodId == Guid.Empty)
            throw new ArgumentException("Fiscal period ID is required.");

        if (journal.Lines.Count == 0)
            throw new InvalidOperationException("Journal has no lines.");

        var groupedLines = journal.Lines
            .GroupBy(line => new
            {
                line.AccountId,
                CurrencyCode = line.Debit.CurrencyCode
            });

        foreach (var group in groupedLines
            .OrderBy(group => group.Key.AccountId)
            .ThenBy(group => group.Key.CurrencyCode, StringComparer.Ordinal))
        {
            // Lock this account balance before reading or creating it.
            await _balanceRepository.AcquireBalanceLockAsync(
                journal.TenantId,
                journal.AccountingBookId,
                fiscalPeriodId,
                group.Key.AccountId,
                group.Key.CurrencyCode,
                cancellationToken);

            // Retrieve the existing balance after acquiring the lock.
            var balance = await _balanceRepository.GetAsync(
                journal.TenantId,
                journal.AccountingBookId,
                fiscalPeriodId,
                group.Key.AccountId,
                group.Key.CurrencyCode,
                cancellationToken);

            // Create a new balance if one does not exist.
            if (balance is null)
            {
                balance = new LedgerAccountBalance(
                    journal.TenantId,
                    journal.CompanyId,
                    journal.AccountingBookId,
                    fiscalPeriodId,
                    group.Key.AccountId,
                    group.Key.CurrencyCode);

                await _balanceRepository.AddAsync(
                    balance,
                    cancellationToken);
            }

            // Calculate debit and credit movements for this account.
            var debitTotal = group.Sum(line => line.Debit.Amount);
            var creditTotal = group.Sum(line => line.Credit.Amount);

            if (debitTotal > 0)
            {
                balance.RecordMovement(debitTotal, 0);
            }

            if (creditTotal > 0)
            {
                balance.RecordMovement(0, creditTotal);
            }
        }
        // await _balanceRepository.SaveChangesAsync(cancellationToken);
    }
}
