namespace Accounting.AccountsPayable.Application.Suppliers.DTOs;

public sealed class SupplierListResponse
{
    public IReadOnlyList<SupplierResponse> Items { get; init; }
        = Array.Empty<SupplierResponse>();

    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(
        (double)TotalCount / PageSize);
}