using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJournalAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "journal_audit_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<int>(type: "integer", nullable: false),
                    previous_status = table.Column<int>(type: "integer", nullable: true),
                    new_status = table.Column<int>(type: "integer", nullable: true),
                    comments = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journal_audit_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_journal_audit_logs_journal_entry_journal_entry_id",
                        column: x => x.journal_entry_id,
                        principalTable: "journal_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_journal_audit_logs_journal_entry_id",
                table: "journal_audit_logs",
                column: "journal_entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_audit_logs_tenant_id_journal_entry_id_occurred_at_u",
                table: "journal_audit_logs",
                columns: new[] { "tenant_id", "journal_entry_id", "occurred_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_journal_audit_logs_tenant_id_user_id",
                table: "journal_audit_logs",
                columns: new[] { "tenant_id", "user_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "journal_audit_logs");
        }
    }
}
