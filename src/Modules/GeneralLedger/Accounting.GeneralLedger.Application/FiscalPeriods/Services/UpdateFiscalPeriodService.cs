using Accounting.Application.Abstractions;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;

namespace Accounting.GeneralLedger.Application.FiscalPeriods.Services;

public sealed class UpdateFiscalPeriodService
{
    private readonly IFiscalPeriodRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFiscalPeriodService(
        IFiscalPeriodRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        Guid fiscalPeriodId,
        Guid tenantId,
        string name,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        if (fiscalPeriodId == Guid.Empty)
            throw new ArgumentException("Fiscal period ID is required.");

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var period = await _repository.GetByIdForUpdateAsync(
                    fiscalPeriodId,
                    tenantId,
                    ct);

                if (period is null)
                    throw new KeyNotFoundException(
                        "Fiscal period was not found.");

                var hasOverlap = await _repository.HasOverlapAsync(
                    tenantId,
                    period.CompanyId,
                    period.AccountingBookId,
                    startDate,
                    endDate,
                    period.Id,
                    ct);

                if (hasOverlap)
                    throw new InvalidOperationException(
                        "The fiscal period overlaps an existing period.");

                period.Rename(name);
                period.UpdateDateRange(startDate, endDate);

                await _repository.SaveChangesAsync(ct);

                return true;
            },
            cancellationToken);
    }
}