
using Accounting.Domain.Organization.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");

        builder.HasKey(c => c.Id);
        
        builder.HasAlternateKey(c => new { c.TenantId, c.Id });

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.LegalName)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(c => c.TaxRegistrationNumber)
            .HasMaxLength(100);

        builder.Property(c => c.CountryCode)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(c => c.BaseCurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .IsRequired();

        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(c => c.TenantId);

        builder.HasIndex(c => new { c.TenantId, c.Code })
            .IsUnique();
    }
}
