using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations.LogsDb
{
    /// <inheritdoc />
    public partial class AddEventTypeToNotificationsLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "event_type",
                table: "notifications_log",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_log_event_type",
                table: "notifications_log",
                column: "event_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_log_event_type",
                table: "notifications_log");

            migrationBuilder.DropColumn(
                name: "event_type",
                table: "notifications_log");
        }
    }
}
