
using Accounting.Application.Abstractions;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;
using Accounting.GeneralLedger.Domain.Ledger.Interfaces;

namespace Accounting.GeneralLedger.Application.FiscalPeriods.Services;

public sealed class UpdateFiscalPeriodStatusService
{
    private readonly IFiscalPeriodRepository _repository;
    private readonly IJournalEntryRepository _journalRepository;
    private readonly ILedgerAccountBalanceRepository _balanceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFiscalPeriodStatusService(
        IFiscalPeriodRepository repository,
        IJournalEntryRepository journalRepository,
        ILedgerAccountBalanceRepository balanceRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _journalRepository = journalRepository;
        _balanceRepository = balanceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        Guid fiscalPeriodId,
        Guid tenantId,
        string action,
        CancellationToken cancellationToken = default)
    {
        if (fiscalPeriodId == Guid.Empty)
            throw new ArgumentException("Fiscal period ID is required.");

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Status action is required.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var period = await _repository.GetByIdAsync(
                    fiscalPeriodId,
                    tenantId,
                    ct);

                if (period is null)
                    throw new KeyNotFoundException(
                        "Fiscal period was not found.");

                var normalizedAction = action.Trim().ToLowerInvariant();

                if (normalizedAction == "close")
                {
                    var hasUnpostedJournals =
                        await _journalRepository.HasUnpostedJournalsAsync(
                            tenantId,
                            period.CompanyId,
                            period.AccountingBookId,
                            period.StartDate,
                            period.EndDate,
                            ct);

                    if (hasUnpostedJournals)
                    {
                        throw new InvalidOperationException(
                            "The fiscal period cannot be closed because " +
                            "draft, pending-approval, or approved journals " +
                            "remain unposted.");
                    }

                    var balances = await _balanceRepository.GetForFiscalPeriodAsync(
                        tenantId,
                        period.CompanyId,
                        period.AccountingBookId,
                        period.Id,
                        ct);

                    if (balances.Any(balance => !balance.IsValid()))
                    {
                        throw new InvalidOperationException(
                            "The fiscal period cannot be closed because one or more " +
                            "ledger account balances are invalid.");
                    }

                    var unbalancedCurrency = balances
                        .GroupBy(balance => balance.CurrencyCode)
                        .FirstOrDefault(group =>
                            group.Sum(balance => balance.ClosingDebit) !=
                            group.Sum(balance => balance.ClosingCredit));

                    if (unbalancedCurrency is not null)
                    {
                        throw new InvalidOperationException(
                            $"The fiscal period cannot be closed because closing debits " +
                            $"and credits do not balance for {unbalancedCurrency.Key}.");
                    }
                }

                switch (normalizedAction)
                {
                    case "open":
                        period.Open();
                        break;

                    case "close":
                        period.Close();
                        break;

                    case "lock":
                        period.Lock();
                        break;

                    case "reopen":
                        period.Reopen();
                        break;

                    default:
                        throw new ArgumentException(
                            "Unsupported fiscal-period action.");
                }

                await _repository.SaveChangesAsync(ct);

                return true;
            },
            cancellationToken);
    }
}
