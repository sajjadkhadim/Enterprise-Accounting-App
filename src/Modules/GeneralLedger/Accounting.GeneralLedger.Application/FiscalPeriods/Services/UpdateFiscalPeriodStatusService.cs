using Accounting.Application.Abstractions;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;

namespace Accounting.GeneralLedger.Application.FiscalPeriods.Services;

public sealed class UpdateFiscalPeriodStatusService
{
    private readonly IFiscalPeriodRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFiscalPeriodStatusService(
        IFiscalPeriodRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
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

                switch (action.Trim().ToLowerInvariant())
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