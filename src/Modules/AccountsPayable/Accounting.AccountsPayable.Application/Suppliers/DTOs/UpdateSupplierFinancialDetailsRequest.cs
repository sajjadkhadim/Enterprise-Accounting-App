using System.ComponentModel.DataAnnotations;

namespace Accounting.AccountsPayable.Application.Suppliers.DTOs;

public sealed class UpdateSupplierFinancialDetailsRequest
{
    [Required, StringLength(3, MinimumLength = 3)]
    public string CurrencyCode { get; set; } = string.Empty;

    [Range(0, 365)]
    public int PaymentTermsDays { get; set; }

    [Range(typeof(decimal), "0", "999999999999999")]
    public decimal CreditLimit { get; set; }
}