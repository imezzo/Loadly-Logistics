using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBroadcastNotificationIdToNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "broadcast_notification_id",
                table: "notifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_broadcast_notification_id",
                table: "notifications",
                column: "broadcast_notification_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_broadcast_notification_id",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "broadcast_notification_id",
                table: "notifications");
        }
    }
}
