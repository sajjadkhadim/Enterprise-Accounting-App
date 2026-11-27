using Accounting.AccountsPayable.Domain.Suppliers.Entities;
using Accounting.AccountsPayable.Domain.Suppliers.Repositories;

namespace Accounting.AccountsPayable.Application.Suppliers.Services;

public sealed class GetSupplierService
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSupplierService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<Supplier> ExecuteAsync(
        Guid supplierId,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        if (supplierId == Guid.Empty)
            throw new ArgumentException("Supplier ID is required.");

        var supplier = await _supplierRepository.GetByIdAsync(
            supplierId,
            tenantId,
            companyId,
            cancellationToken);

        return supplier ?? throw new KeyNotFoundException(
            "Supplier was not found.");
    }
}