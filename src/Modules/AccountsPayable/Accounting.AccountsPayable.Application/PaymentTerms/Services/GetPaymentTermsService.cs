using Accounting.AccountsPayable.Application.PaymentTerms.DTOs;
using Accounting.AccountsPayable.Domain.PaymentTerms.Repositories;

namespace Accounting.AccountsPayable.Application.PaymentTerms.Services;

public sealed class GetPaymentTermsService
{
    private readonly IPaymentTermRepository _repository;

    public GetPaymentTermsService(IPaymentTermRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PaymentTermResponse>> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(
            tenantId, companyId, cancellationToken);

        return items.Select(PaymentTermResponse.FromEntity).ToList();
    }
}