using Accounting.AccountsPayable.Domain.PaymentTerms.Entities;
using Accounting.AccountsPayable.Domain.PaymentTerms.Repositories;

namespace Accounting.AccountsPayable.Application.PaymentTerms.Services;

public sealed class CreatePaymentTermService
{
    private readonly IPaymentTermRepository _repository;

    public CreatePaymentTermService(IPaymentTermRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        string code,
        string name,
        int dueDays,
        int? discountDays = null,
        decimal? discountPercentage = null,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty || companyId == Guid.Empty)
            throw new ArgumentException("Tenant and company are required.");

        code = code.Trim().ToUpperInvariant();

        if (await _repository.ExistsByCodeAsync(
            code, tenantId, companyId, cancellationToken))
        {
            throw new InvalidOperationException(
                "A payment term with this code already exists.");
        }

        var paymentTerm = new PaymentTerm(
            tenantId,
            companyId,
            code,
            name,
            dueDays,
            discountDays,
            discountPercentage);

        await _repository.AddAsync(paymentTerm, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return paymentTerm.Id;
    }
}