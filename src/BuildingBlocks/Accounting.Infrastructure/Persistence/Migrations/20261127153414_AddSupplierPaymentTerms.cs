using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierPaymentTerms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "payment_term_id",
                table: "suppliers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "payment_terms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    due_days = table.Column<int>(type: "integer", nullable: false),
                    discount_days = table.Column<int>(type: "integer", nullable: true),
                    discount_percentage = table.Column<decimal>(type: "numeric(7,4)", precision: 7, scale: 4, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_terms", x => x.id);
                    table.CheckConstraint("ck_payment_terms_discount", "(discount_days IS NULL AND discount_percentage IS NULL) OR (discount_days >= 0 AND discount_days <= due_days AND discount_percentage > 0 AND discount_percentage <= 100)");
                    table.CheckConstraint("ck_payment_terms_due_days", "due_days >= 0 AND due_days <= 365");
                });

            migrationBuilder.CreateIndex(
                name: "ix_suppliers_payment_term_id",
                table: "suppliers",
                column: "payment_term_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_terms_tenant_id_company_id_code",
                table: "payment_terms",
                columns: new[] { "tenant_id", "company_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_payment_terms_tenant_id_company_id_status",
                table: "payment_terms",
                columns: new[] { "tenant_id", "company_id", "status" });

            migrationBuilder.AddForeignKey(
                name: "fk_suppliers_payment_terms_payment_term_id",
                table: "suppliers",
                column: "payment_term_id",
                principalTable: "payment_terms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_suppliers_payment_terms_payment_term_id",
                table: "suppliers");

            migrationBuilder.DropTable(
                name: "payment_terms");

            migrationBuilder.DropIndex(
                name: "ix_suppliers_payment_term_id",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "payment_term_id",
                table: "suppliers");
        }
    }
}
