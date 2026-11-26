using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StrengthenGeneralLedgerRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_journal_entries_accounting_books_accounting_book_id",
                table: "journal_entries");

            migrationBuilder.DropForeignKey(
                name: "fk_journal_entries_companies_company_id",
                table: "journal_entries");

            migrationBuilder.DropForeignKey(
                name: "fk_ledger_account_balances_accounting_books_accounting_book_id",
                table: "ledger_account_balances");

            migrationBuilder.DropForeignKey(
                name: "fk_ledger_account_balances_accounts_account_id",
                table: "ledger_account_balances");

            migrationBuilder.DropForeignKey(
                name: "fk_ledger_account_balances_companies_company_id",
                table: "ledger_account_balances");

            migrationBuilder.DropForeignKey(
                name: "fk_ledger_account_balances_fiscal_periods_fiscal_period_id",
                table: "ledger_account_balances");

            migrationBuilder.DropIndex(
                name: "ix_ledger_account_balances_account_id",
                table: "ledger_account_balances");

            migrationBuilder.DropIndex(
                name: "ix_ledger_account_balances_accounting_book_id",
                table: "ledger_account_balances");

            migrationBuilder.DropIndex(
                name: "ix_ledger_account_balances_company_id",
                table: "ledger_account_balances");

            migrationBuilder.DropIndex(
                name: "ix_ledger_account_balances_fiscal_period_id",
                table: "ledger_account_balances");

            migrationBuilder.DropIndex(
                name: "ix_journal_entries_accounting_book_id",
                table: "journal_entries");

            migrationBuilder.DropIndex(
                name: "ix_journal_entries_company_id",
                table: "journal_entries");

            migrationBuilder.DropIndex(
                name: "ix_fiscal_periods_tenant_id_company_id_accounting_book_id",
                table: "fiscal_periods");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_fiscal_periods_tenant_id_company_id_accounting_book_id_id",
                table: "fiscal_periods",
                columns: new[] { "tenant_id", "company_id", "accounting_book_id", "id" });

            migrationBuilder.AddUniqueConstraint(
                name: "ak_accounts_tenant_id_id",
                table: "accounts",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.CreateIndex(
                name: "ix_ledger_account_balances_tenant_id_account_id",
                table: "ledger_account_balances",
                columns: new[] { "tenant_id", "account_id" });

            migrationBuilder.CreateIndex(
                name: "ix_ledger_account_balances_tenant_id_company_id_accounting_boo",
                table: "ledger_account_balances",
                columns: new[] { "tenant_id", "company_id", "accounting_book_id", "fiscal_period_id" });

            migrationBuilder.AddCheckConstraint(
                name: "ck_journal_lines_matching_currencies",
                table: "journal_lines",
                sql: "debit_currency_code = credit_currency_code");

            migrationBuilder.AddCheckConstraint(
                name: "ck_journal_lines_valid_debit_credit",
                table: "journal_lines",
                sql: "(debit_amount > 0 AND credit_amount = 0)\r\nOR\r\n(credit_amount > 0 AND debit_amount = 0)");

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_tenant_id_company_id_accounting_book_id",
                table: "journal_entries",
                columns: new[] { "tenant_id", "company_id", "accounting_book_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_journal_entries_accounting_books_tenant_id_company_id_accou",
                table: "journal_entries",
                columns: new[] { "tenant_id", "company_id", "accounting_book_id" },
                principalTable: "accounting_books",
                principalColumns: new[] { "tenant_id", "company_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_journal_entries_companies_tenant_id_company_id",
                table: "journal_entries",
                columns: new[] { "tenant_id", "company_id" },
                principalTable: "companies",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ledger_account_balances_accounting_books_tenant_id_company_",
                table: "ledger_account_balances",
                columns: new[] { "tenant_id", "company_id", "accounting_book_id" },
                principalTable: "accounting_books",
                principalColumns: new[] { "tenant_id", "company_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ledger_account_balances_accounts_tenant_id_account_id",
                table: "ledger_account_balances",
                columns: new[] { "tenant_id", "account_id" },
                principalTable: "accounts",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ledger_account_balances_companies_tenant_id_company_id",
                table: "ledger_account_balances",
                columns: new[] { "tenant_id", "company_id" },
                principalTable: "companies",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ledger_account_balances_fiscal_periods_tenant_id_company_id",
                table: "ledger_account_balances",
                columns: new[] { "tenant_id", "company_id", "accounting_book_id", "fiscal_period_id" },
                principalTable: "fiscal_periods",
                principalColumns: new[] { "tenant_id", "company_id", "accounting_book_id", "id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_journal_entries_accounting_books_tenant_id_company_id_accou",
                table: "journal_entries");

            migrationBuilder.DropForeignKey(
                name: "fk_journal_entries_companies_tenant_id_company_id",
                table: "journal_entries");

            migrationBuilder.DropForeignKey(
                name: "fk_ledger_account_balances_accounting_books_tenant_id_company_",
                table: "ledger_account_balances");

            migrationBuilder.DropForeignKey(
                name: "fk_ledger_account_balances_accounts_tenant_id_account_id",
                table: "ledger_account_balances");

            migrationBuilder.DropForeignKey(
                name: "fk_ledger_account_balances_companies_tenant_id_company_id",
                table: "ledger_account_balances");

            migrationBuilder.DropForeignKey(
                name: "fk_ledger_account_balances_fiscal_periods_tenant_id_company_id",
                table: "ledger_account_balances");

            migrationBuilder.DropIndex(
                name: "ix_ledger_account_balances_tenant_id_account_id",
                table: "ledger_account_balances");

            migrationBuilder.DropIndex(
                name: "ix_ledger_account_balances_tenant_id_company_id_accounting_boo",
                table: "ledger_account_balances");

            migrationBuilder.DropCheckConstraint(
                name: "ck_journal_lines_matching_currencies",
                table: "journal_lines");

            migrationBuilder.DropCheckConstraint(
                name: "ck_journal_lines_valid_debit_credit",
                table: "journal_lines");

            migrationBuilder.DropIndex(
                name: "ix_journal_entries_tenant_id_company_id_accounting_book_id",
                table: "journal_entries");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_fiscal_periods_tenant_id_company_id_accounting_book_id_id",
                table: "fiscal_periods");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_accounts_tenant_id_id",
                table: "accounts");

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
                name: "ix_journal_entries_accounting_book_id",
                table: "journal_entries",
                column: "accounting_book_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_company_id",
                table: "journal_entries",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_periods_tenant_id_company_id_accounting_book_id",
                table: "fiscal_periods",
                columns: new[] { "tenant_id", "company_id", "accounting_book_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_journal_entries_accounting_books_accounting_book_id",
                table: "journal_entries",
                column: "accounting_book_id",
                principalTable: "accounting_books",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_journal_entries_companies_company_id",
                table: "journal_entries",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ledger_account_balances_accounting_books_accounting_book_id",
                table: "ledger_account_balances",
                column: "accounting_book_id",
                principalTable: "accounting_books",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ledger_account_balances_accounts_account_id",
                table: "ledger_account_balances",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ledger_account_balances_companies_company_id",
                table: "ledger_account_balances",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ledger_account_balances_fiscal_periods_fiscal_period_id",
                table: "ledger_account_balances",
                column: "fiscal_period_id",
                principalTable: "fiscal_periods",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
