using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChartOfAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "ak_accounting_books_tenant_id_id",
                table: "accounting_books",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.CreateTable(
                name: "charts_of_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    accounting_book_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_charts_of_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_charts_of_accounts_accounting_books_tenant_id_accounting_bo",
                        columns: x => new { x.tenant_id, x.accounting_book_id },
                        principalTable: "accounting_books",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_charts_of_accounts_tenant_id",
                table: "charts_of_accounts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_charts_of_accounts_tenant_id_accounting_book_id_code",
                table: "charts_of_accounts",
                columns: new[] { "tenant_id", "accounting_book_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "charts_of_accounts");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_accounting_books_tenant_id_id",
                table: "accounting_books");
        }
    }
}
