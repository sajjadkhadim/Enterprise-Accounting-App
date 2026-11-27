
using Accounting.AccountsPayable.Domain.Bills.Repositories;

namespace Accounting.AccountsPayable.Application.Bills.Services;

public sealed class RejectSupplierBillService
{
    private readonly ISupplierBillRepository _repository;

    public RejectSupplierBillService(ISupplierBillRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(
        Guid billId,
        Guid tenantId,
        Guid companyId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("A rejection reason is required.");

        var bill = await _repository.GetByIdAsync(
            billId, tenantId, companyId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier bill not found.");

        bill.Reject();

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
