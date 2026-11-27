
using Accounting.AccountsPayable.Domain.Configuration.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.AccountsPayable.Infrastructure.Configuration;

public sealed class AccountsPayableSettingsConfiguration
    : IEntityTypeConfiguration<AccountsPayableSettings>
{
    public void Configure(EntityTypeBuilder<AccountsPayableSettings> builder)
    {
        builder.ToTable("accounts_payable_settings", table =>
        {
            table.HasCheckConstraint(
                "ck_ap_settings_different_accounts",
                "input_vat_account_id IS NULL OR " +
                "input_vat_account_id <> payable_control_account_id");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.AccountingBookId).IsRequired();

        builder.Property(x => x.PayableControlAccountId).IsRequired();
        builder.Property(x => x.InputVatAccountId);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.CompanyId,
            x.AccountingBookId
        }).IsUnique();

        builder.HasOne<Accounting.Domain.Organization.Entities.Company>()
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Accounting.Domain.GeneralLedger.Entities.AccountingBook>()
            .WithMany()
            .HasForeignKey(x => x.AccountingBookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Accounting.Domain.ChartOfAccounts.Entities.Account>()
            .WithMany()
            .HasForeignKey(x => x.PayableControlAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Accounting.Domain.ChartOfAccounts.Entities.Account>()
            .WithMany()
            .HasForeignKey(x => x.InputVatAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
