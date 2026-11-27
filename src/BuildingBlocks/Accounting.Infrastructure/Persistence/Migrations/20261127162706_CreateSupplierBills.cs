using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateSupplierBills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "supplier_bills",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    accounting_book_id = table.Column<Guid>(type: "uuid", nullable: false),
                    supplier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bill_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    supplier_invoice_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    invoice_date = table.Column<DateOnly>(type: "date", nullable: false),
                    due_date = table.Column<DateOnly>(type: "date", nullable: false),
                    currency_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_supplier_bills", x => x.id);
                    table.CheckConstraint("ck_supplier_bills_currency", "char_length(currency_code) = 3");
                    table.CheckConstraint("ck_supplier_bills_due_date", "due_date >= invoice_date");
                    table.ForeignKey(
                        name: "fk_supplier_bills_accounting_books_accounting_book_id",
                        column: x => x.accounting_book_id,
                        principalTable: "accounting_books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_supplier_bills_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_supplier_bills_supplier_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supplier_bill_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric(7,4)", precision: 7, scale: 4, nullable: false),
                    supplier_bill_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_supplier_bill_lines", x => x.id);
                    table.CheckConstraint("ck_supplier_bill_lines_quantity", "quantity > 0");
                    table.CheckConstraint("ck_supplier_bill_lines_tax_rate", "tax_rate >= 0 AND tax_rate <= 100");
                    table.CheckConstraint("ck_supplier_bill_lines_unit_price", "unit_price >= 0");
                    table.ForeignKey(
                        name: "fk_supplier_bill_lines_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_supplier_bill_lines_supplier_bills_supplier_bill_id",
                        column: x => x.supplier_bill_id,
                        principalTable: "supplier_bills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_supplier_bill_lines_account_id",
                table: "supplier_bill_lines",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_supplier_bill_lines_supplier_bill_id",
                table: "supplier_bill_lines",
                column: "supplier_bill_id");

            migrationBuilder.CreateIndex(
                name: "ix_supplier_bills_accounting_book_id",
                table: "supplier_bills",
                column: "accounting_book_id");

            migrationBuilder.CreateIndex(
                name: "ix_supplier_bills_company_id",
                table: "supplier_bills",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_supplier_bills_supplier_id",
                table: "supplier_bills",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "ix_supplier_bills_tenant_id_company_id_accounting_book_id_bill",
                table: "supplier_bills",
                columns: new[] { "tenant_id", "company_id", "accounting_book_id", "bill_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_supplier_bills_tenant_id_company_id_accounting_book_id_invo",
                table: "supplier_bills",
                columns: new[] { "tenant_id", "company_id", "accounting_book_id", "invoice_date" });

            migrationBuilder.CreateIndex(
                name: "ix_supplier_bills_tenant_id_company_id_supplier_id_supplier_in",
                table: "supplier_bills",
                columns: new[] { "tenant_id", "company_id", "supplier_id", "supplier_invoice_number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "supplier_bill_lines");

            migrationBuilder.DropTable(
                name: "supplier_bills");
        }
    }
}
