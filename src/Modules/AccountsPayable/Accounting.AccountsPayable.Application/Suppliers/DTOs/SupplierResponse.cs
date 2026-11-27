using Accounting.AccountsPayable.Domain.Suppliers.Entities;
using Accounting.AccountsPayable.Domain.Suppliers.Enums;

namespace Accounting.AccountsPayable.Application.Suppliers.DTOs;

public sealed class SupplierResponse
{
    public Guid Id { get; init; }
    public Guid CompanyId { get; init; }
    public string SupplierCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? LegalName { get; init; }
    public string? TaxRegistrationNumber { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? CountryCode { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public int PaymentTermsDays { get; init; }
    public decimal CreditLimit { get; init; }
    public SupplierStatus Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }

    public static SupplierResponse FromEntity(Supplier supplier)
    {
        return new SupplierResponse
        {
            Id = supplier.Id,
            CompanyId = supplier.CompanyId,
            SupplierCode = supplier.SupplierCode,
            Name = supplier.Name,
            LegalName = supplier.LegalName,
            TaxRegistrationNumber = supplier.TaxRegistrationNumber,
            Email = supplier.Email,
            Phone = supplier.Phone,
            Address = supplier.Address,
            CountryCode = supplier.CountryCode,
            CurrencyCode = supplier.CurrencyCode,
            PaymentTermsDays = supplier.PaymentTermsDays,
            CreditLimit = supplier.CreditLimit,
            Status = supplier.Status,
            CreatedAtUtc = supplier.CreatedAtUtc
        };
    }
}