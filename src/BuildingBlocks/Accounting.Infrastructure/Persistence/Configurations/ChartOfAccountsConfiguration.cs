
using Accounting.Domain.ChartOfAccounts.Entities;
using Accounting.Domain.GeneralLedger.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations;

public class ChartOfAccountsConfiguration
    : IEntityTypeConfiguration<ChartOfAccounts>
{
    public void Configure(EntityTypeBuilder<ChartOfAccounts> builder)
    {
        builder.ToTable("charts_of_accounts");

        builder.HasKey(c => c.Id);

        builder.HasAlternateKey(c => new { c.TenantId, c.Id });

        builder.Property(c => c.AccountingBookId)
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.IsActive)
            .IsRequired();

        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(c => c.TenantId);

        builder.HasIndex(c => new
        {
            c.TenantId,
            c.AccountingBookId,
            c.Code
        })
            .IsUnique();

        builder.HasOne<AccountingBook>()
            .WithMany()
            .HasForeignKey(c => new
            {
                c.TenantId,
                c.AccountingBookId
            })
            .HasPrincipalKey(b => new
            {
                b.TenantId,
                b.Id
            })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
