using Accounting.AccountsPayable.Domain.PaymentTerms.Entities;

namespace Accounting.AccountsPayable.Domain.PaymentTerms.Repositories;

public interface IPaymentTermRepository
{
    Task<PaymentTerm?> GetByIdAsync(
        Guid id,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PaymentTerm paymentTerm,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentTerm>> GetAllAsync(
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}