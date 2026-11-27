using Accounting.AccountsPayable.Domain.Suppliers.Repositories;

namespace Accounting.AccountsPayable.Application.Suppliers.Services;

public sealed class UpdateSupplierService
{
    private readonly ISupplierRepository _supplierRepository;

    public UpdateSupplierService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task ExecuteAsync(
        Guid supplierId,
        Guid tenantId,
        Guid companyId,
        string name,
        string? legalName,
        string? taxRegistrationNumber,
        string? email,
        string? phone,
        string? address,
        string? countryCode,
        CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(
            supplierId,
            tenantId,
            companyId,
            cancellationToken);

        if (supplier is null)
            throw new KeyNotFoundException("Supplier was not found.");

        supplier.UpdateDetails(
            name,
            legalName,
            taxRegistrationNumber,
            email,
            phone,
            address,
            countryCode);

        await _supplierRepository.SaveChangesAsync(cancellationToken);
    }
}