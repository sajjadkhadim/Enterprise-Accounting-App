
using Accounting.Domain.ChartOfAccounts.Entities;
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.GeneralLedger.Infrastructure.Persistence.Configurations;

public sealed class JournalLineConfiguration
    : IEntityTypeConfiguration<JournalLine>
{
    public void Configure(EntityTypeBuilder<JournalLine> builder)
    {
        builder.ToTable("journal_lines");

        builder.HasKey(line => line.Id);

        builder.Property(line => line.JournalEntryId)
            .IsRequired();

        builder.Property(line => line.AccountId)
            .IsRequired();

        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(line => line.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(line => line.Description)
            .HasMaxLength(500);

        builder.OwnsOne(line => line.Debit, money =>
        {
            money.Property(value => value.Amount)
                .HasColumnName("debit_amount")
                .HasPrecision(19, 4)
                .IsRequired();

            money.Property(value => value.CurrencyCode)
                .HasColumnName("debit_currency_code")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(line => line.Credit, money =>
        {
            money.Property(value => value.Amount)
                .HasColumnName("credit_amount")
                .HasPrecision(19, 4)
                .IsRequired();

            money.Property(value => value.CurrencyCode)
                .HasColumnName("credit_currency_code")
                .HasMaxLength(3)
                .IsRequired();
        });
    }
}
