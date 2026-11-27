
using Accounting.Application.Abstractions;
using Accounting.Domain.Common.Interfaces;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Enums;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;
using Accounting.GeneralLedger.Domain.Ledger.Entities;
using Accounting.GeneralLedger.Domain.Ledger.Interfaces;

namespace Accounting.GeneralLedger.Application.FiscalPeriods.Services;

public sealed class CarryForwardFiscalPeriodBalancesService
{
    private readonly IFiscalPeriodRepository _fiscalPeriodRepository;
    private readonly ILedgerAccountBalanceRepository _balanceRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork;

    public CarryForwardFiscalPeriodBalancesService(
        IFiscalPeriodRepository fiscalPeriodRepository,
        ILedgerAccountBalanceRepository balanceRepository,
        ITenantContext tenantContext,
        IUnitOfWork unitOfWork)
    {
        _fiscalPeriodRepository = fiscalPeriodRepository;
        _balanceRepository = balanceRepository;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> ExecuteAsync(
        Guid targetFiscalPeriodId,
        CancellationToken cancellationToken = default)
    {
        if (targetFiscalPeriodId == Guid.Empty)
            throw new ArgumentException("Target fiscal period ID is required.");

        var tenantId = _tenantContext.TenantId;

        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
            throw new InvalidOperationException(
                "An active tenant context is required.");

        return await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var targetPeriod =
                    await _fiscalPeriodRepository.GetByIdForUpdateAsync(
                        targetFiscalPeriodId,
                        tenantId.Value,
                        ct);

                if (targetPeriod is null)
                    throw new InvalidOperationException(
                        "Target fiscal period was not found.");

                if (targetPeriod.Status != FiscalPeriodStatus.Future)
                    throw new InvalidOperationException(
                        "Carry-forward is allowed only for future fiscal periods.");

                var periods =
                    await _fiscalPeriodRepository.GetAllAsync(
                        tenantId.Value,
                        targetPeriod.CompanyId,
                        targetPeriod.AccountingBookId,
                        ct);

                var previousPeriod = periods
                    .Where(p =>
                        p.Id != targetPeriod.Id &&
                        p.StartDate < targetPeriod.StartDate &&
                        p.EndDate < targetPeriod.StartDate)
                    .OrderByDescending(p => p.StartDate)
                    .FirstOrDefault();

                if (previousPeriod is null)
                    throw new InvalidOperationException(
                        "No previous fiscal period was found.");

                if (previousPeriod.Status != FiscalPeriodStatus.Closed &&
                    previousPeriod.Status != FiscalPeriodStatus.Locked)
                {
                    throw new InvalidOperationException(
                        "The previous fiscal period must be closed or locked.");
                }

                var existingTargetBalances =
                    await _balanceRepository.GetForFiscalPeriodAsync(
                        tenantId.Value,
                        targetPeriod.CompanyId,
                        targetPeriod.AccountingBookId,
                        targetPeriod.Id,
                        ct);

                if (existingTargetBalances.Count > 0)
                    throw new InvalidOperationException(
                        "The target fiscal period already contains balances. " +
                        "Carry-forward cannot overwrite existing balances.");

                var previousBalances =
                    await _balanceRepository.GetForFiscalPeriodAsync(
                        tenantId.Value,
                        previousPeriod.CompanyId,
                        previousPeriod.AccountingBookId,
                        previousPeriod.Id,
                        ct);

                var carriedCount = 0;

                foreach (var previousBalance in previousBalances)
                {
                    if (!previousBalance.IsValid())
                        throw new InvalidOperationException(
                            $"Previous balance for account " +
                            $"{previousBalance.AccountId} is invalid.");

                    // Zero balances do not need a destination record.
                    if (previousBalance.ClosingDebit == 0 &&
                        previousBalance.ClosingCredit == 0)
                    {
                        continue;
                    }

                    var newBalance = new LedgerAccountBalance(
                        tenantId.Value,
                        targetPeriod.CompanyId,
                        targetPeriod.AccountingBookId,
                        targetPeriod.Id,
                        previousBalance.AccountId,
                        previousBalance.CurrencyCode);

                    newBalance.SetOpeningBalance(
                        previousBalance.ClosingDebit,
                        previousBalance.ClosingCredit);

                    await _balanceRepository.AddAsync(newBalance, ct);

                    carriedCount++;
                }

                await _balanceRepository.SaveChangesAsync(ct);

                return carriedCount;
            },
            cancellationToken);
    }
}
