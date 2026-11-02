using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations.LogsDb
{
    /// <inheritdoc />
    public partial class AddAuditLogEntityTypeCreatedAtIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_entity_type_created_at",
                table: "audit_logs",
                columns: new[] { "entity_type", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_audit_logs_entity_type_created_at",
                table: "audit_logs");
        }
    }
}
