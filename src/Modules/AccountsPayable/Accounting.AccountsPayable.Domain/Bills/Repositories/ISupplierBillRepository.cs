using Accounting.AccountsPayable.Domain.Bills.Entities;
using Accounting.AccountsPayable.Domain.Suppliers.Entities;

namespace Accounting.AccountsPayable.Domain.Bills.Repositories;

public interface ISupplierBillRepository
{
    Task<bool> SupplierExistsAsync(
        Guid supplierId,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> AccountingBookExistsAsync(
        Guid accountingBookId,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> AccountsAreValidAsync(
        IReadOnlyCollection<Guid> accountIds,
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        CancellationToken cancellationToken = default);

    Task<Supplier?> GetSupplierAsync(
        Guid supplierId,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> BillNumberExistsAsync(
        string billNumber,
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        CancellationToken cancellationToken = default);

    Task<SupplierBill?> GetByIdAsync(
        Guid billId,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        SupplierBill bill,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}