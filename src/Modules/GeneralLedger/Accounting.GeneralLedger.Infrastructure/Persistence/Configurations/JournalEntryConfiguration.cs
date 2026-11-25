
using Accounting.Domain.GeneralLedger.Entities;
using Accounting.Domain.Organization.Entities;
using Accounting.Domain.Tenancy.Entities;
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.GeneralLedger.Infrastructure.Persistence.Configurations;

public sealed class JournalEntryConfiguration
    : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("journal_entries",
            table =>
            {
                table.HasCheckConstraint(
                    "ck_journal_entries_company_id",
                    "company_id <> '00000000-0000-0000-0000-000000000000'");

                table.HasCheckConstraint(
                    "ck_journal_entries_accounting_book_id",
                    "accounting_book_id <> '00000000-0000-0000-0000-000000000000'");

                table.HasCheckConstraint(
                    "ck_journal_entries_journal_number",
                    "length(trim(journal_number)) > 0");

                table.HasCheckConstraint(
                    "ck_journal_entries_description_length",
                    "description IS NULL OR length(description) <= 500");
            });

        builder.HasKey(journal => journal.Id);

        builder.Property<uint>("xmin")
            .IsRowVersion();

        builder.Property(journal => journal.CompanyId)
            .IsRequired();

        builder.Property(journal => journal.AccountingBookId)
            .IsRequired();

        builder.Property(journal => journal.JournalNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(journal => journal.Description)
            .HasMaxLength(500);

        builder.Property(journal => journal.JournalDate)
            .IsRequired();

        builder.Property(journal => journal.JournalType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(journal => journal.JournalSource)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(journal => journal.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(journal => journal.CreatedAtUtc)
            .IsRequired();

        // Tenant relationship
        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(journal => journal.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Company relationship
        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(journal => journal.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Accounting book relationship
        builder.HasOne<AccountingBook>()
            .WithMany()
            .HasForeignKey(journal => journal.AccountingBookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(journal => journal.Lines)
            .WithOne()
            .HasForeignKey(line => line.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(journal => journal.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(journal => new
        {
            journal.TenantId,
            journal.AccountingBookId,
            journal.JournalNumber
        })
        .IsUnique();

        builder.HasIndex(journal => new
        {
            journal.TenantId,
            journal.CompanyId,
            journal.JournalDate
        });


    }
}
