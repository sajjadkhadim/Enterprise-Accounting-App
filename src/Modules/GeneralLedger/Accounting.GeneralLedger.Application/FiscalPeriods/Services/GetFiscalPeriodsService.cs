using Accounting.GeneralLedger.Domain.FiscalPeriods.Entities;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;

namespace Accounting.GeneralLedger.Application.FiscalPeriods.Services;

public sealed class GetFiscalPeriodsService
{
    private readonly IFiscalPeriodRepository _repository;

    public GetFiscalPeriodsService(
        IFiscalPeriodRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<FiscalPeriod>> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required.");

        if (accountingBookId == Guid.Empty)
            throw new ArgumentException("Accounting book ID is required.");

        return _repository.GetAllAsync(
            tenantId,
            companyId,
            accountingBookId,
            cancellationToken);
    }
}