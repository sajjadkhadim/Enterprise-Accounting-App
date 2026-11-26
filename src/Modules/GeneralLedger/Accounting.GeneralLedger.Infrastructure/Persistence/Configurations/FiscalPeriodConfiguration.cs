using Accounting.Domain.GeneralLedger.Entities;
using Accounting.Domain.Organization.Entities;
using Accounting.Domain.Tenancy.Entities;
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

        builder.HasAlternateKey(period => new
        {
            period.TenantId,
            period.CompanyId,
            period.AccountingBookId,
            period.Id
        });

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

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(period => period.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(period => new
            {
                period.TenantId,
                period.CompanyId
            })
            .HasPrincipalKey(company => new
            {
                company.TenantId,
                company.Id
            })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AccountingBook>()
            .WithMany()
            .HasForeignKey(period => new
            {
                period.TenantId,
                period.CompanyId,
                period.AccountingBookId
            })
            .HasPrincipalKey(book => new
            {
                book.TenantId,
                book.CompanyId,
                book.Id
            })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(period => new
        {
            period.TenantId,
            period.AccountingBookId,
            period.StartDate,
            period.EndDate
        });
    }
}