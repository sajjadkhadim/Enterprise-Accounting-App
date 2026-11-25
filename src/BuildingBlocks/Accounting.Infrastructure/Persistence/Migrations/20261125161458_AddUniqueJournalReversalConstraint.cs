using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueJournalReversalConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_tenant_id_reverses_journal_entry_id",
                table: "journal_entries",
                columns: new[] { "tenant_id", "reverses_journal_entry_id" },
                unique: true,
                filter: "reverses_journal_entry_id IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_journal_entries_tenant_id_reverses_journal_entry_id",
                table: "journal_entries");
        }
    }
}
