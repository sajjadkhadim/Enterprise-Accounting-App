using Accounting.GeneralLedger.Domain.FiscalPeriods.Entities;

namespace Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;

public interface IFiscalPeriodRepository
{
    Task<FiscalPeriod?> GetByIdAsync(
    Guid fiscalPeriodId,
    Guid tenantId,
    CancellationToken cancellationToken = default);

    Task<FiscalPeriod?> GetForDateAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FiscalPeriod>> GetAllAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        CancellationToken cancellationToken = default);

    Task<bool> HasOverlapAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludeFiscalPeriodId = null,
        CancellationToken cancellationToken = default);


    Task AddAsync(
        FiscalPeriod fiscalPeriod,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}