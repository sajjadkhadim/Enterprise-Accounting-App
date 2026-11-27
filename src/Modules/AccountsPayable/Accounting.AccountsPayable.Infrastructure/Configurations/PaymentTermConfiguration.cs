using Accounting.AccountsPayable.Domain.PaymentTerms.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.AccountsPayable.Infrastructure.Configurations;

public sealed class PaymentTermConfiguration
    : IEntityTypeConfiguration<PaymentTerm>
{
    public void Configure(EntityTypeBuilder<PaymentTerm> builder)
    {
        builder.ToTable("payment_terms", table =>
        {
            table.HasCheckConstraint(
                "ck_payment_terms_due_days",
                "due_days >= 0 AND due_days <= 365");

            table.HasCheckConstraint(
                "ck_payment_terms_discount",
                "(discount_days IS NULL AND discount_percentage IS NULL) OR " +
                "(discount_days >= 0 AND discount_days <= due_days " +
                "AND discount_percentage > 0 AND discount_percentage <= 100)");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DueDays)
            .IsRequired();

        builder.Property(x => x.DiscountPercentage)
            .HasPrecision(7, 4);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.CompanyId,
            x.Code
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.CompanyId,
            x.Status
        });
    }
}