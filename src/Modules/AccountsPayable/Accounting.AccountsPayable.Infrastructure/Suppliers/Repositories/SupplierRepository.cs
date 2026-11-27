using Accounting.AccountsPayable.Domain.Suppliers.Entities;
using Accounting.AccountsPayable.Domain.Suppliers.Enums;
using Accounting.AccountsPayable.Domain.Suppliers.Repositories;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounting.AccountsPayable.Infrastructure.Suppliers.Repositories;

public sealed class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _context;

    public SupplierRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Supplier?> GetByIdAsync(
        Guid supplierId,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<Supplier>()
            .FirstOrDefaultAsync(
                x => x.Id == supplierId
                    && x.TenantId == tenantId
                    && x.CompanyId == companyId,
                cancellationToken);
    }

    public Task<Supplier?> GetByCodeAsync(
        string supplierCode,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        supplierCode = supplierCode.Trim().ToUpperInvariant();

        return _context.Set<Supplier>()
            .FirstOrDefaultAsync(
                x => x.SupplierCode == supplierCode
                    && x.TenantId == tenantId
                    && x.CompanyId == companyId,
                cancellationToken);
    }

    public async Task<(IReadOnlyList<Supplier> Items, int TotalCount)> SearchAsync(
        Guid tenantId,
        Guid companyId,
        string? search,
        SupplierStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Supplier>()
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                x.CompanyId == companyId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";

            query = query.Where(x =>
                EF.Functions.ILike(x.SupplierCode, term) ||
                EF.Functions.ILike(x.Name, term));
        }

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.SupplierCode)
            .ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task AddAsync(
        Supplier supplier,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<Supplier>().AddAsync(
            supplier, cancellationToken).AsTask();
    }

    public Task<bool> ExistsByCodeAsync(
        string supplierCode,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        supplierCode = supplierCode.Trim().ToUpperInvariant();

        return _context.Set<Supplier>()
            .AnyAsync(
                x => x.SupplierCode == supplierCode
                    && x.TenantId == tenantId
                    && x.CompanyId == companyId,
                cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}