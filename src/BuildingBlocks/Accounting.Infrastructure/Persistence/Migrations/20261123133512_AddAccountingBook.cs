using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "ak_companies_tenant_id_id",
                table: "companies",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.CreateTable(
                name: "accounting_books",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    accounting_basis = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounting_books", x => x.id);
                    table.ForeignKey(
                        name: "fk_accounting_books_companies_tenant_id_company_id",
                        columns: x => new { x.tenant_id, x.company_id },
                        principalTable: "companies",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accounting_books_tenant_id",
                table: "accounting_books",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounting_books_tenant_id_company_id_code",
                table: "accounting_books",
                columns: new[] { "tenant_id", "company_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounting_books");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_companies_tenant_id_id",
                table: "companies");
        }
    }
}
