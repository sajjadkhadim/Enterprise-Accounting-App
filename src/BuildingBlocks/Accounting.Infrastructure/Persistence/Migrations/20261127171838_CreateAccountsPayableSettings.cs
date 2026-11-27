using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateAccountsPayableSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts_payable_settings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    accounting_book_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payable_control_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    input_vat_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounts_payable_settings", x => x.id);
                    table.CheckConstraint("ck_ap_settings_different_accounts", "input_vat_account_id IS NULL OR input_vat_account_id <> payable_control_account_id");
                    table.ForeignKey(
                        name: "fk_accounts_payable_settings_accounting_books_accounting_book_",
                        column: x => x.accounting_book_id,
                        principalTable: "accounting_books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accounts_payable_settings_accounts_input_vat_account_id",
                        column: x => x.input_vat_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accounts_payable_settings_accounts_payable_control_account_",
                        column: x => x.payable_control_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accounts_payable_settings_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_payable_settings_accounting_book_id",
                table: "accounts_payable_settings",
                column: "accounting_book_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_payable_settings_company_id",
                table: "accounts_payable_settings",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_payable_settings_input_vat_account_id",
                table: "accounts_payable_settings",
                column: "input_vat_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_payable_settings_payable_control_account_id",
                table: "accounts_payable_settings",
                column: "payable_control_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_payable_settings_tenant_id_company_id_accounting_b",
                table: "accounts_payable_settings",
                columns: new[] { "tenant_id", "company_id", "accounting_book_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounts_payable_settings");
        }
    }
}
