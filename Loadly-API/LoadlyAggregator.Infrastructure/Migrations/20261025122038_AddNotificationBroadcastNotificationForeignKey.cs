using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationBroadcastNotificationForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_notifications_broadcast_notifications_broadcast_notificatio~",
                table: "notifications",
                column: "broadcast_notification_id",
                principalTable: "broadcast_notifications",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_broadcast_notifications_broadcast_notificatio~",
                table: "notifications");
        }
    }
}
