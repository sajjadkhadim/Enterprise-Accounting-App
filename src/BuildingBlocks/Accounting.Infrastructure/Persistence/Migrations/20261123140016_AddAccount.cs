using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "ak_charts_of_accounts_tenant_id_id",
                table: "charts_of_accounts",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chart_of_accounts_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    account_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    account_category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_posting_allowed = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounts", x => x.id);
                    table.UniqueConstraint("ak_accounts_tenant_id_chart_of_accounts_id_id", x => new { x.tenant_id, x.chart_of_accounts_id, x.id });
                    table.CheckConstraint("ck_accounts_code_not_empty", "length(trim(code)) > 0");
                    table.CheckConstraint("ck_accounts_level_positive", "level >= 1");
                    table.ForeignKey(
                        name: "fk_accounts_accounts_tenant_id_chart_of_accounts_id_parent_acc",
                        columns: x => new { x.tenant_id, x.chart_of_accounts_id, x.parent_account_id },
                        principalTable: "accounts",
                        principalColumns: new[] { "tenant_id", "chart_of_accounts_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accounts_charts_of_accounts_tenant_id_chart_of_accounts_id",
                        columns: x => new { x.tenant_id, x.chart_of_accounts_id },
                        principalTable: "charts_of_accounts",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_tenant_id_chart_of_accounts_id",
                table: "accounts",
                columns: new[] { "tenant_id", "chart_of_accounts_id" });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_tenant_id_chart_of_accounts_id_parent_account_id",
                table: "accounts",
                columns: new[] { "tenant_id", "chart_of_accounts_id", "parent_account_id" });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_tenant_id_code",
                table: "accounts",
                columns: new[] { "tenant_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_charts_of_accounts_tenant_id_id",
                table: "charts_of_accounts");
        }
    }
}
