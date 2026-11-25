
using Accounting.Domain.ChartOfAccounts.Entities;
using Accounting.Domain.GeneralLedger.Entities;
using Accounting.Domain.Organization.Entities;
using Accounting.Domain.Tenancy.Entities;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Entities;
using Accounting.GeneralLedger.Domain.Ledger.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.GeneralLedger.Infrastructure.Persistence.Configurations;

public sealed class LedgerAccountBalanceConfiguration
    : IEntityTypeConfiguration<LedgerAccountBalance>
{
    public void Configure(
        EntityTypeBuilder<LedgerAccountBalance> builder)
    {
        builder.ToTable("ledger_account_balances", table =>
        {
            table.HasCheckConstraint(
                "ck_ledger_balances_currency_code",
                "char_length(currency_code) = 3");

            table.HasCheckConstraint(
                "ck_ledger_balances_amounts_non_negative",
                """
                opening_debit >= 0 AND
                opening_credit >= 0 AND
                period_debit >= 0 AND
                period_credit >= 0 AND
                closing_debit >= 0 AND
                closing_credit >= 0
                """);

            table.HasCheckConstraint(
                "ck_ledger_balances_opening_sides",
                "NOT (opening_debit > 0 AND opening_credit > 0)");

            table.HasCheckConstraint(
                "ck_ledger_balances_closing_sides",
                "NOT (closing_debit > 0 AND closing_credit > 0)");

            table.HasCheckConstraint(
                "ck_ledger_balances_closing_calculation",
                """
                closing_debit - closing_credit =
                opening_debit - opening_credit +
                period_debit - period_credit
                """);
        });

        builder.HasKey(balance => balance.Id);

        builder.Property<uint>("xmin")
            .IsRowVersion();

        builder.Property(balance => balance.CompanyId).IsRequired();
        builder.Property(balance => balance.AccountingBookId).IsRequired();
        builder.Property(balance => balance.FiscalPeriodId).IsRequired();
        builder.Property(balance => balance.AccountId).IsRequired();

        builder.Property(balance => balance.CurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(balance => balance.OpeningDebit)
            .HasPrecision(19, 4);

        builder.Property(balance => balance.OpeningCredit)
            .HasPrecision(19, 4);

        builder.Property(balance => balance.PeriodDebit)
            .HasPrecision(19, 4);

        builder.Property(balance => balance.PeriodCredit)
            .HasPrecision(19, 4);

        builder.Property(balance => balance.ClosingDebit)
            .HasPrecision(19, 4);

        builder.Property(balance => balance.ClosingCredit)
            .HasPrecision(19, 4);

        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(balance => balance.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<FiscalPeriod>()
            .WithMany()
            .HasForeignKey(balance => balance.FiscalPeriodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(balance => balance.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AccountingBook>()
            .WithMany()
            .HasForeignKey(balance => balance.AccountingBookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(balance => balance.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(balance => new
        {
            balance.TenantId,
            balance.AccountingBookId,
            balance.FiscalPeriodId,
            balance.AccountId,
            balance.CurrencyCode
        }).IsUnique();

        builder.HasIndex(balance => new
        {
            balance.TenantId,
            balance.CompanyId,
            balance.FiscalPeriodId
        });

    }
}
