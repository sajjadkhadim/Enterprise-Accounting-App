using Accounting.AccountsPayable.Domain.Bills.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.AccountsPayable.Infrastructure.Configurations;

public sealed class SupplierBillConfiguration
    : IEntityTypeConfiguration<SupplierBill>
{
    public void Configure(EntityTypeBuilder<SupplierBill> builder)
    {
        builder.ToTable("supplier_bills", table =>
        {
            table.HasCheckConstraint(
                "ck_supplier_bills_due_date",
                "due_date >= invoice_date");

            table.HasCheckConstraint(
                "ck_supplier_bills_currency",
                "char_length(currency_code) = 3");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BillNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.SupplierInvoiceNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.InvoiceDate)
            .IsRequired();

        builder.Property(x => x.DueDate)
            .IsRequired();

        builder.Property(x => x.CurrencyCode)
            .HasMaxLength(3)
            .IsFixedLength()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey("SupplierBillId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Lines)
            .HasField("_lines")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.CompanyId,
            x.AccountingBookId,
            x.BillNumber
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.CompanyId,
            x.SupplierId,
            x.SupplierInvoiceNumber
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.CompanyId,
            x.AccountingBookId,
            x.InvoiceDate
        });

        builder.HasOne<Accounting.Domain.Organization.Entities.Company>()
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Accounting.Domain.GeneralLedger.Entities.AccountingBook>()
            .WithMany()
            .HasForeignKey(x => x.AccountingBookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Accounting.AccountsPayable.Domain.Suppliers.Entities.Supplier>()
            .WithMany()
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}