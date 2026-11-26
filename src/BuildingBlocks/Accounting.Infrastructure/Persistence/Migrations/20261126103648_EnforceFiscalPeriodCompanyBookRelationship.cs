using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnforceFiscalPeriodCompanyBookRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_fiscal_periods_accounting_books_accounting_book_id",
                table: "fiscal_periods");

            migrationBuilder.DropForeignKey(
                name: "fk_fiscal_periods_companies_company_id",
                table: "fiscal_periods");

            migrationBuilder.DropIndex(
                name: "ix_fiscal_periods_accounting_book_id",
                table: "fiscal_periods");

            migrationBuilder.DropIndex(
                name: "ix_fiscal_periods_company_id",
                table: "fiscal_periods");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_accounting_books_tenant_id_company_id_id",
                table: "accounting_books",
                columns: new[] { "tenant_id", "company_id", "id" });

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_periods_tenant_id_company_id_accounting_book_id",
                table: "fiscal_periods",
                columns: new[] { "tenant_id", "company_id", "accounting_book_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_fiscal_periods_accounting_books_tenant_id_company_id_accoun",
                table: "fiscal_periods",
                columns: new[] { "tenant_id", "company_id", "accounting_book_id" },
                principalTable: "accounting_books",
                principalColumns: new[] { "tenant_id", "company_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fiscal_periods_companies_tenant_id_company_id",
                table: "fiscal_periods",
                columns: new[] { "tenant_id", "company_id" },
                principalTable: "companies",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_fiscal_periods_accounting_books_tenant_id_company_id_accoun",
                table: "fiscal_periods");

            migrationBuilder.DropForeignKey(
                name: "fk_fiscal_periods_companies_tenant_id_company_id",
                table: "fiscal_periods");

            migrationBuilder.DropIndex(
                name: "ix_fiscal_periods_tenant_id_company_id_accounting_book_id",
                table: "fiscal_periods");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_accounting_books_tenant_id_company_id_id",
                table: "accounting_books");

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_periods_accounting_book_id",
                table: "fiscal_periods",
                column: "accounting_book_id");

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_periods_company_id",
                table: "fiscal_periods",
                column: "company_id");

            migrationBuilder.AddForeignKey(
                name: "fk_fiscal_periods_accounting_books_accounting_book_id",
                table: "fiscal_periods",
                column: "accounting_book_id",
                principalTable: "accounting_books",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fiscal_periods_companies_company_id",
                table: "fiscal_periods",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
