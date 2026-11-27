using Accounting.AccountsPayable.Domain.PaymentTerms.Entities;

namespace Accounting.AccountsPayable.Application.PaymentTerms.DTOs;

public sealed class PaymentTermResponse
{
    public Guid Id { get; init; }
    public Guid CompanyId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int DueDays { get; init; }
    public int? DiscountDays { get; init; }
    public decimal? DiscountPercentage { get; init; }
    public int Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }

    public static PaymentTermResponse FromEntity(PaymentTerm entity)
    {
        return new PaymentTermResponse
        {
            Id = entity.Id,
            CompanyId = entity.CompanyId,
            Code = entity.Code,
            Name = entity.Name,
            DueDays = entity.DueDays,
            DiscountDays = entity.DiscountDays,
            DiscountPercentage = entity.DiscountPercentage,
            Status = (int)entity.Status,
            CreatedAtUtc = entity.CreatedAtUtc
        };
    }
}