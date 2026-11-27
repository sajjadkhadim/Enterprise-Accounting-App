using Accounting.AccountsPayable.Application.Suppliers.DTOs;
using Accounting.AccountsPayable.Domain.Suppliers.Enums;
using Accounting.AccountsPayable.Domain.Suppliers.Repositories;

namespace Accounting.AccountsPayable.Application.Suppliers.Services;

public sealed class SearchSuppliersService
{
    private readonly ISupplierRepository _repository;

    public SearchSuppliersService(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<SupplierListResponse> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        string? search,
        SupplierStatus? status,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty || companyId == Guid.Empty)
            throw new ArgumentException("Tenant and company are required.");

        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page));

        if (pageSize < 1 || pageSize > 100)
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size must be between 1 and 100.");

        var result = await _repository.SearchAsync(
            tenantId,
            companyId,
            search,
            status,
            page,
            pageSize,
            cancellationToken);

        return new SupplierListResponse
        {
            Items = result.Items
                .Select(SupplierResponse.FromEntity)
                .ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount
        };
    }
}