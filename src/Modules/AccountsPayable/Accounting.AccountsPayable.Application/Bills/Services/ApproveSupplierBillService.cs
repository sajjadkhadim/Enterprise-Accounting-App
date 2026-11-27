
using Accounting.AccountsPayable.Domain.Bills.Repositories;

namespace Accounting.AccountsPayable.Application.Bills.Services;

public sealed class ApproveSupplierBillService
{
    private readonly ISupplierBillRepository _repository;

    public ApproveSupplierBillService(ISupplierBillRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(
        Guid billId,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var bill = await _repository.GetByIdAsync(
            billId, tenantId, companyId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier bill not found.");

        bill.Approve();

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
