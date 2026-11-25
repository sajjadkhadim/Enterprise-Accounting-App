using Accounting.Application.Abstractions;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Entities;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;

namespace Accounting.GeneralLedger.Application.FiscalPeriods.Services;

public sealed class CreateFiscalPeriodService
{
    private readonly IFiscalPeriodRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFiscalPeriodService(
        IFiscalPeriodRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        FiscalPeriod fiscalPeriod,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fiscalPeriod);

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        if (fiscalPeriod.TenantId != tenantId)
            throw new InvalidOperationException(
                "The fiscal period tenant does not match the authenticated tenant.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var hasOverlap = await _repository.HasOverlapAsync(
                    tenantId,
                    fiscalPeriod.CompanyId,
                    fiscalPeriod.AccountingBookId,
                    fiscalPeriod.StartDate,
                    fiscalPeriod.EndDate,
                    null,
                    ct);

                if (hasOverlap)
                    throw new InvalidOperationException(
                        "The fiscal period overlaps an existing period.");

                await _repository.AddAsync(fiscalPeriod, ct);
                await _repository.SaveChangesAsync(ct);

                return true;
            },
            cancellationToken);
    }
}