using Accounting.AccountsPayable.Domain.PaymentTerms.Repositories;

namespace Accounting.AccountsPayable.Application.PaymentTerms.Services;

public sealed class ChangePaymentTermStatusService
{
    private readonly IPaymentTermRepository _repository;

    public ChangePaymentTermStatusService(IPaymentTermRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(
        Guid id,
        Guid tenantId,
        Guid companyId,
        string action,
        CancellationToken cancellationToken = default)
    {
        var paymentTerm = await _repository.GetByIdAsync(
            id, tenantId, companyId, cancellationToken)
            ?? throw new KeyNotFoundException("Payment term not found.");

        switch (action.Trim().ToLowerInvariant())
        {
            case "activate":
                paymentTerm.Activate();
                break;

            case "deactivate":
                paymentTerm.Deactivate();
                break;

            default:
                throw new ArgumentException(
                    "Action must be activate or deactivate.");
        }

        await _repository.SaveChangesAsync(cancellationToken);
    }
}