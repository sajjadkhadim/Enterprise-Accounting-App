using Accounting.AccountsPayable.Domain.PaymentTerms.Repositories;

namespace Accounting.AccountsPayable.Application.PaymentTerms.Services;

public sealed class UpdatePaymentTermService
{
    private readonly IPaymentTermRepository _repository;

    public UpdatePaymentTermService(IPaymentTermRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(
        Guid id,
        Guid tenantId,
        Guid companyId,
        string name,
        int dueDays,
        int? discountDays,
        decimal? discountPercentage,
        CancellationToken cancellationToken = default)
    {
        var paymentTerm = await _repository.GetByIdAsync(
            id, tenantId, companyId, cancellationToken)
            ?? throw new KeyNotFoundException("Payment term not found.");

        if (paymentTerm.Status !=
            Accounting.AccountsPayable.Domain.PaymentTerms.Enums.PaymentTermStatus.Active)
        {
            throw new InvalidOperationException(
                "Only active payment terms can be updated.");
        }

        paymentTerm.Update(
            name, dueDays, discountDays, discountPercentage);

        await _repository.SaveChangesAsync(cancellationToken);
    }
}