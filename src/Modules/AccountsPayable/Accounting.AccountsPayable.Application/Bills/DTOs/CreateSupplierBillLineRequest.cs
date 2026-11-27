using System.ComponentModel.DataAnnotations;

namespace Accounting.AccountsPayable.Application.Bills.DTOs;

public sealed class CreateSupplierBillLineRequest
{
    [Required]
    public Guid AccountId { get; set; }

    [Required, MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.0001", "999999999999999")]
    public decimal Quantity { get; set; }

    [Range(typeof(decimal), "0", "999999999999999")]
    public decimal UnitPrice { get; set; }

    [Range(typeof(decimal), "0", "100")]
    public decimal TaxRate { get; set; }
}