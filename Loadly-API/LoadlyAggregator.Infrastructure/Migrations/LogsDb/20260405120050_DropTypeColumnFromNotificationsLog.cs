using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations.LogsDb
{
    /// <inheritdoc />
    public partial class DropTypeColumnFromNotificationsLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_log_type",
                table: "notifications_log");

            migrationBuilder.DropIndex(
                name: "IX_notifications_log_type_status_created_at",
                table: "notifications_log");

            migrationBuilder.DropColumn(
                name: "type",
                table: "notifications_log");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_log_recipient_type_status_created_at",
                table: "notifications_log",
                columns: new[] { "recipient_type", "status", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_log_recipient_type_status_created_at",
                table: "notifications_log");

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "notifications_log",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_log_type",
                table: "notifications_log",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_log_type_status_created_at",
                table: "notifications_log",
                columns: new[] { "type", "status", "created_at" });
        }
    }
}
