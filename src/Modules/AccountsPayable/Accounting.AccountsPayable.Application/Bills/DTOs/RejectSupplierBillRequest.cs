
using System.ComponentModel.DataAnnotations;

namespace Accounting.AccountsPayable.Application.Bills.DTOs;

public sealed class RejectSupplierBillRequest
{
    [Required]
    [MaxLength(1000)]
    public string Reason { get; set; } = string.Empty;
}
