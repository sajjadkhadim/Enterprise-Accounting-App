using Accounting.AccountsPayable.Domain.Suppliers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.AccountsPayable.Infrastructure.Configurations;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers", table =>
        {
            table.HasCheckConstraint(
                "ck_suppliers_payment_terms_days",
                "payment_terms_days >= 0 AND payment_terms_days <= 365");

            table.HasCheckConstraint(
                "ck_suppliers_credit_limit",
                "credit_limit >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SupplierCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.LegalName).HasMaxLength(200);
        builder.Property(x => x.TaxRegistrationNumber).HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(254);
        builder.Property(x => x.Phone).HasMaxLength(50);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.CountryCode).HasMaxLength(2);

        builder.Property(x => x.CurrencyCode)
            .HasMaxLength(3)
            .IsFixedLength()
            .IsRequired();

        builder.Property(x => x.PaymentTermsDays).IsRequired();

        builder.Property(x => x.CreditLimit)
            .HasPrecision(19, 4)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.PaymentTermId);

        builder.Property(x => x.CreatedAtUtc).IsRequired();


        builder.HasOne<Accounting.AccountsPayable.Domain.PaymentTerms.Entities.PaymentTerm>()
            .WithMany()
            .HasForeignKey(x => x.PaymentTermId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasIndex(x => new
        {
            x.TenantId,
            x.CompanyId,
            x.SupplierCode
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.CompanyId,
            x.Name
        });

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.CompanyId,
            x.Status
        });


    }
}