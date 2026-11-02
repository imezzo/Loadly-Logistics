using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations.LogsDb
{
    /// <inheritdoc />
    public partial class AddBroadcastNotificationIdToNotificationsLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "broadcast_notification_id",
                table: "notifications_log",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_log_broadcast_notification_id",
                table: "notifications_log",
                column: "broadcast_notification_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_log_broadcast_notification_id",
                table: "notifications_log");

            migrationBuilder.DropColumn(
                name: "broadcast_notification_id",
                table: "notifications_log");
        }
    }
}
