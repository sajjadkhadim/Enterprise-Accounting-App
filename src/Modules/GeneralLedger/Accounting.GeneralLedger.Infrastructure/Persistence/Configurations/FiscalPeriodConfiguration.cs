
using Accounting.GeneralLedger.Domain.FiscalPeriods.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.GeneralLedger.Infrastructure.Persistence.Configurations;

public sealed class FiscalPeriodConfiguration
    : IEntityTypeConfiguration<FiscalPeriod>
{
    public void Configure(EntityTypeBuilder<FiscalPeriod> builder)
    {
        builder.ToTable("fiscal_periods", table =>
        {
            table.HasCheckConstraint(
                "ck_fiscal_periods_date_range",
                "end_date >= start_date");

            table.HasCheckConstraint(
                "ck_fiscal_periods_company_id",
                "company_id <> '00000000-0000-0000-0000-000000000000'");

            table.HasCheckConstraint(
                "ck_fiscal_periods_accounting_book_id",
                "accounting_book_id <> '00000000-0000-0000-0000-000000000000'");
        });

        builder.HasKey(period => period.Id);

        builder.Property(period => period.CompanyId)
            .IsRequired();

        builder.Property(period => period.AccountingBookId)
            .IsRequired();

        builder.Property(period => period.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(period => period.StartDate)
            .IsRequired();

        builder.Property(period => period.EndDate)
            .IsRequired();

        builder.Property(period => period.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(period => period.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(period => new
        {
            period.TenantId,
            period.AccountingBookId,
            period.StartDate,
            period.EndDate
        });
    }
}
