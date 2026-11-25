using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFiscalPeriodOverlapConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fiscal_periods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    accounting_book_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fiscal_periods", x => x.id);
                    table.CheckConstraint("ck_fiscal_periods_accounting_book_id", "accounting_book_id <> '00000000-0000-0000-0000-000000000000'");
                    table.CheckConstraint("ck_fiscal_periods_company_id", "company_id <> '00000000-0000-0000-0000-000000000000'");
                    table.CheckConstraint("ck_fiscal_periods_date_range", "end_date >= start_date");
                    table.ForeignKey(
                        name: "fk_fiscal_periods_accounting_books_accounting_book_id",
                        column: x => x.accounting_book_id,
                        principalTable: "accounting_books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_fiscal_periods_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_fiscal_periods_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

                migrationBuilder.Sql(
                    """
                    CREATE EXTENSION IF NOT EXISTS btree_gist;

                    ALTER TABLE fiscal_periods
                    ADD CONSTRAINT ex_fiscal_periods_no_overlap
                    EXCLUDE USING gist
                    (
                        tenant_id WITH =,
                        company_id WITH =,
                        accounting_book_id WITH =,
                        daterange(start_date, end_date, '[]') WITH &&
                    );
                """);
                
            migrationBuilder.CreateTable(
                name: "journal_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    accounting_book_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reverses_journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reversed_by_journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    journal_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    journal_date = table.Column<DateOnly>(type: "date", nullable: false),
                    journal_type = table.Column<int>(type: "integer", nullable: false),
                    journal_source = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journal_entries", x => x.id);
                    table.CheckConstraint("ck_journal_entries_accounting_book_id", "accounting_book_id <> '00000000-0000-0000-0000-000000000000'");
                    table.CheckConstraint("ck_journal_entries_company_id", "company_id <> '00000000-0000-0000-0000-000000000000'");
                    table.CheckConstraint("ck_journal_entries_description_length", "description IS NULL OR length(description) <= 500");
                    table.CheckConstraint("ck_journal_entries_journal_number", "length(trim(journal_number)) > 0");
                    table.ForeignKey(
                        name: "fk_journal_entries_accounting_books_accounting_book_id",
                        column: x => x.accounting_book_id,
                        principalTable: "accounting_books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_journal_entries_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_journal_entries_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ledger_account_balances",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    accounting_book_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fiscal_period_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    opening_debit = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    opening_credit = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    period_debit = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    period_credit = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    closing_debit = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    closing_credit = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ledger_account_balances", x => x.id);
                    table.CheckConstraint("ck_ledger_balances_amounts_non_negative", "opening_debit >= 0 AND\r\nopening_credit >= 0 AND\r\nperiod_debit >= 0 AND\r\nperiod_credit >= 0 AND\r\nclosing_debit >= 0 AND\r\nclosing_credit >= 0");
                    table.CheckConstraint("ck_ledger_balances_closing_calculation", "closing_debit - closing_credit =\r\nopening_debit - opening_credit +\r\nperiod_debit - period_credit");
                    table.CheckConstraint("ck_ledger_balances_closing_sides", "NOT (closing_debit > 0 AND closing_credit > 0)");
                    table.CheckConstraint("ck_ledger_balances_currency_code", "char_length(currency_code) = 3");
                    table.CheckConstraint("ck_ledger_balances_opening_sides", "NOT (opening_debit > 0 AND opening_credit > 0)");
                    table.ForeignKey(
                        name: "fk_ledger_account_balances_accounting_books_accounting_book_id",
                        column: x => x.accounting_book_id,
                        principalTable: "accounting_books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ledger_account_balances_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ledger_account_balances_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ledger_account_balances_fiscal_periods_fiscal_period_id",
                        column: x => x.fiscal_period_id,
                        principalTable: "fiscal_periods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ledger_account_balances_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "journal_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    debit_amount = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    debit_currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    credit_amount = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    credit_currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journal_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_journal_lines_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_journal_lines_journal_entries_journal_entry_id",
                        column: x => x.journal_entry_id,
                        principalTable: "journal_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_periods_accounting_book_id",
                table: "fiscal_periods",
                column: "accounting_book_id");

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_periods_company_id",
                table: "fiscal_periods",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_periods_tenant_id_accounting_book_id_start_date_end_",
                table: "fiscal_periods",
                columns: new[] { "tenant_id", "accounting_book_id", "start_date", "end_date" });

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_accounting_book_id",
                table: "journal_entries",
                column: "accounting_book_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_company_id",
                table: "journal_entries",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_tenant_id_accounting_book_id_journal_number",
                table: "journal_entries",
                columns: new[] { "tenant_id", "accounting_book_id", "journal_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_tenant_id_company_id_journal_date",
                table: "journal_entries",
                columns: new[] { "tenant_id", "company_id", "journal_date" });

            migrationBuilder.CreateIndex(
                name: "ix_journal_lines_account_id",
                table: "journal_lines",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_lines_journal_entry_id",
                table: "journal_lines",
                column: "journal_entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_ledger_account_balances_account_id",
                table: "ledger_account_balances",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_ledger_account_balances_accounting_book_id",
                table: "ledger_account_balances",
                column: "accounting_book_id");

            migrationBuilder.CreateIndex(
                name: "ix_ledger_account_balances_company_id",
                table: "ledger_account_balances",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_ledger_account_balances_fiscal_period_id",
                table: "ledger_account_balances",
                column: "fiscal_period_id");

            migrationBuilder.CreateIndex(
                name: "ix_ledger_account_balances_tenant_id_accounting_book_id_fiscal",
                table: "ledger_account_balances",
                columns: new[] { "tenant_id", "accounting_book_id", "fiscal_period_id", "account_id", "currency_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ledger_account_balances_tenant_id_company_id_fiscal_period_",
                table: "ledger_account_balances",
                columns: new[] { "tenant_id", "company_id", "fiscal_period_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "journal_lines");

            migrationBuilder.DropTable(
                name: "ledger_account_balances");

            migrationBuilder.DropTable(
                name: "journal_entries");

            migrationBuilder.DropTable(
                name: "fiscal_periods");
        }
    }
}
