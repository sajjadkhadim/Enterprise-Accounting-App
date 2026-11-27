using Accounting.AccountsPayable.Domain.Suppliers.Entities;
using Accounting.AccountsPayable.Domain.Suppliers.Repositories;

namespace Accounting.AccountsPayable.Application.Suppliers.Services;

public sealed class CreateSupplierService
{
    private readonly ISupplierRepository _supplierRepository;

    public CreateSupplierService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<Guid> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        string supplierCode,
        string name,
        string currencyCode,
        int paymentTermsDays = 30,
        decimal creditLimit = 0,
        string? legalName = null,
        string? taxRegistrationNumber = null,
        string? email = null,
        string? phone = null,
        string? address = null,
        string? countryCode = null,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required.");

        supplierCode = supplierCode.Trim().ToUpperInvariant();

        if (await _supplierRepository.ExistsByCodeAsync(
            supplierCode, tenantId, companyId, cancellationToken))
        {
            throw new InvalidOperationException(
                "A supplier with this code already exists for this company.");
        }

        var supplier = new Supplier(
            tenantId,
            companyId,
            supplierCode,
            name,
            currencyCode,
            paymentTermsDays,
            creditLimit);

        supplier.UpdateDetails(
            name,
            legalName,
            taxRegistrationNumber,
            email,
            phone,
            address,
            countryCode);

        await _supplierRepository.AddAsync(supplier, cancellationToken);
        await _supplierRepository.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}