
using Accounting.Domain.ChartOfAccounts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations;

public sealed class AccountConfiguration
    : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts", table =>
        {
            table.HasCheckConstraint(
                "ck_accounts_level_positive",
                "level >= 1");

            table.HasCheckConstraint(
                "ck_accounts_code_not_empty",
                "length(trim(code)) > 0");
        });

        builder.HasKey(a => a.Id);

        builder.HasAlternateKey(a => new { a.TenantId, a.Id });

        // Supports the composite self-reference to a parent account.
        builder.HasAlternateKey(a => new
        {
            a.TenantId,
            a.ChartOfAccountsId,
            a.Id
        });

        builder.Property(a => a.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.AccountType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(a => a.AccountCategory)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.CashFlowCategory)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.IsCashOrCashEquivalent)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.IsPostingAllowed).IsRequired();
        builder.Property(a => a.IsActive).IsRequired();
        builder.Property(a => a.Level).IsRequired();
        builder.Property(a => a.CreatedAtUtc).IsRequired();

        // Account codes must be unique across the entire tenant,
        // not merely within one chart.
        builder.HasIndex(a => new { a.TenantId, a.Code })
            .IsUnique();

        builder.HasIndex(a => new
        {
            a.TenantId,
            a.ChartOfAccountsId
        });

        // An account must reference a chart in the same tenant.
        builder.HasOne<ChartOfAccounts>()
            .WithMany()
            .HasForeignKey(a => new
            {
                a.TenantId,
                a.ChartOfAccountsId
            })
            .HasPrincipalKey(c => new
            {
                c.TenantId,
                c.Id
            })
            .OnDelete(DeleteBehavior.Restrict);

        // A parent account must belong to the same chart and tenant.
        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(a => new
            {
                a.TenantId,
                a.ChartOfAccountsId,
                a.ParentAccountId
            })
            .HasPrincipalKey(a => new
            {
                a.TenantId,
                a.ChartOfAccountsId,
                a.Id
            })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
