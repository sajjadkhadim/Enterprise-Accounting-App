using Accounting.AccountsPayable.Domain.Bills.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.AccountsPayable.Infrastructure.Configurations;

public sealed class SupplierBillLineConfiguration
    : IEntityTypeConfiguration<SupplierBillLine>
{
    public void Configure(EntityTypeBuilder<SupplierBillLine> builder)
    {
        builder.ToTable("supplier_bill_lines", table =>
        {
            table.HasCheckConstraint(
                "ck_supplier_bill_lines_quantity",
                "quantity > 0");

            table.HasCheckConstraint(
                "ck_supplier_bill_lines_unit_price",
                "unit_price >= 0");

            table.HasCheckConstraint(
                "ck_supplier_bill_lines_tax_rate",
                "tax_rate >= 0 AND tax_rate <= 100");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasPrecision(19, 4)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasPrecision(19, 4)
            .IsRequired();

        builder.Property(x => x.TaxRate)
            .HasPrecision(7, 4)
            .IsRequired();

        builder.Ignore(x => x.Subtotal);
        builder.Ignore(x => x.TaxAmount);
        builder.Ignore(x => x.Total);

        builder.HasOne<Accounting.Domain.ChartOfAccounts.Entities.Account>()
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AccountId);
    }
}