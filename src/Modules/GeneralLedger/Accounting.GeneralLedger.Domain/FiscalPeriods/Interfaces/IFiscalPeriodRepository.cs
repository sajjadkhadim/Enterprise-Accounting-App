
using Accounting.GeneralLedger.Domain.FiscalPeriods.Entities;

namespace Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;

public interface IFiscalPeriodRepository
{
    Task<FiscalPeriod?> GetForDateAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        DateOnly date,
        CancellationToken cancellationToken = default);
}
