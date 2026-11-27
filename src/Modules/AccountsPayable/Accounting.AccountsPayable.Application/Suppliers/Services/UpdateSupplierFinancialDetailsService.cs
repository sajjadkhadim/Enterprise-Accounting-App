using Accounting.AccountsPayable.Domain.Suppliers.Repositories;

namespace Accounting.AccountsPayable.Application.Suppliers.Services;

public sealed class UpdateSupplierFinancialDetailsService
{
    private readonly ISupplierRepository _supplierRepository;

    public UpdateSupplierFinancialDetailsService(
        ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task ExecuteAsync(
        Guid supplierId,
        Guid tenantId,
        Guid companyId,
        string currencyCode,
        int paymentTermsDays,
        decimal creditLimit,
        CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(
            supplierId,
            tenantId,
            companyId,
            cancellationToken);

        if (supplier is null)
            throw new KeyNotFoundException("Supplier was not found.");

        supplier.UpdateFinancialDetails(
            currencyCode,
            paymentTermsDays,
            creditLimit);

        await _supplierRepository.SaveChangesAsync(cancellationToken);
    }
}