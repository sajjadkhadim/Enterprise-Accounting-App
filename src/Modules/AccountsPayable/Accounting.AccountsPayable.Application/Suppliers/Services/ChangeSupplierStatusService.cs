using Accounting.AccountsPayable.Domain.Suppliers.Repositories;

namespace Accounting.AccountsPayable.Application.Suppliers.Services;

public sealed class ChangeSupplierStatusService
{
    private readonly ISupplierRepository _supplierRepository;

    public ChangeSupplierStatusService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task ExecuteAsync(
        Guid supplierId,
        Guid tenantId,
        Guid companyId,
        string action,
        CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(
            supplierId,
            tenantId,
            companyId,
            cancellationToken);

        if (supplier is null)
            throw new KeyNotFoundException("Supplier was not found.");

        switch (action.Trim().ToLowerInvariant())
        {
            case "activate":
                supplier.Activate();
                break;

            case "deactivate":
                supplier.Deactivate();
                break;

            case "hold":
                supplier.PutOnHold();
                break;

            default:
                throw new ArgumentException("Invalid supplier status action.");
        }

        await _supplierRepository.SaveChangesAsync(cancellationToken);
    }
}