
using Accounting.Domain.GeneralLedger.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations;

public class AccountingBookConfiguration
    : IEntityTypeConfiguration<AccountingBook>
{
    public void Configure(EntityTypeBuilder<AccountingBook> builder)
    {
        builder.ToTable("accounting_books");

        builder.HasKey(b => b.Id);

        builder.HasAlternateKey(b => new { b.TenantId, b.Id });

        builder.Property(b => b.CompanyId)
            .IsRequired();

        builder.Property(b => b.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.AccountingBasis)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.CurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(b => b.IsActive)
            .IsRequired();

        builder.Property(b => b.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(b => b.TenantId);

        builder.HasIndex(b => new { b.TenantId, b.CompanyId, b.Code })
            .IsUnique();

        builder.HasOne<Accounting.Domain.Organization.Entities.Company>()
            .WithMany()
            .HasForeignKey(b => new { b.TenantId, b.CompanyId })
            .HasPrincipalKey(c => new { c.TenantId, c.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
