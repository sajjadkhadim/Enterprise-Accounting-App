
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.GeneralLedger.Infrastructure.Persistence.Configurations;

public sealed class JournalAuditLogConfiguration
    : IEntityTypeConfiguration<JournalAuditLog>
{
    public void Configure(EntityTypeBuilder<JournalAuditLog> builder)
    {
        builder.ToTable("journal_audit_logs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.JournalEntryId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Action)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.PreviousStatus)
            .HasConversion<int?>();

        builder.Property(x => x.NewStatus)
            .HasConversion<int?>();

        builder.Property(x => x.Comments)
            .HasMaxLength(1000);

        builder.Property(x => x.OccurredAtUtc)
            .IsRequired();

        builder.HasOne<JournalEntry>()
            .WithMany()
            .HasForeignKey(x => x.JournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.JournalEntryId,
            x.OccurredAtUtc
        });

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.UserId
        });
    }
}
