using System.ComponentModel.DataAnnotations;

namespace Accounting.AccountsPayable.Application.Bills.DTOs;

public sealed class CreateSupplierBillRequest
{
    [Required]
    public Guid AccountingBookId { get; set; }

    [Required]
    public Guid SupplierId { get; set; }

    [Required, MaxLength(50)]
    public string BillNumber { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string SupplierInvoiceNumber { get; set; } = string.Empty;

    public DateOnly InvoiceDate { get; set; }

    public DateOnly? DueDate { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    public string CurrencyCode { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public List<CreateSupplierBillLineRequest> Lines { get; set; } = new();
}