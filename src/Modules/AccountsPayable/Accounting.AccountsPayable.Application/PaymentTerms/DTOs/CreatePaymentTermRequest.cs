using System.ComponentModel.DataAnnotations;

namespace Accounting.AccountsPayable.Application.PaymentTerms.DTOs;

public sealed class CreatePaymentTermRequest
{
    [Required, MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 365)]
    public int DueDays { get; set; }

    [Range(0, 365)]
    public int? DiscountDays { get; set; }

    [Range(typeof(decimal), "0.0001", "100")]
    public decimal? DiscountPercentage { get; set; }
}