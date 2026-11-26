
using Accounting.GeneralLedger.Domain.Ledger.Entities;

namespace Accounting.GeneralLedger.Domain.Ledger.Interfaces;

public interface ILedgerAccountBalanceRepository
{
    Task<LedgerAccountBalance?> GetAsync(
        Guid tenantId,
        Guid accountingBookId,
        Guid fiscalPeriodId,
        Guid accountId,
        string currencyCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LedgerAccountBalance>> GetForFiscalPeriodAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid fiscalPeriodId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LedgerAccountBalance>> GetTrialBalanceAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid fiscalPeriodId,
        string currencyCode,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        LedgerAccountBalance balance,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
