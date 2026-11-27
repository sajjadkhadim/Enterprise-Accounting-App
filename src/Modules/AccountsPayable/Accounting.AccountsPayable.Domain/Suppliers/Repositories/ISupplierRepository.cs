using Accounting.AccountsPayable.Domain.Suppliers.Entities;
using Accounting.AccountsPayable.Domain.Suppliers.Enums;

namespace Accounting.AccountsPayable.Domain.Suppliers.Repositories;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(
        Guid supplierId,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<Supplier?> GetByCodeAsync(
        string supplierCode,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Supplier> Items, int TotalCount)> SearchAsync(
        Guid tenantId,
        Guid companyId,
        string? search,
        SupplierStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Supplier supplier,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string supplierCode,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}