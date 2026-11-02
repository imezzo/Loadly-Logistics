using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameNotificationEventTypeToEventType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "notification_event_type",
                table: "notifications",
                newName: "event_type");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_notification_event_type",
                table: "notifications",
                newName: "IX_notifications_event_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "event_type",
                table: "notifications",
                newName: "notification_event_type");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_event_type",
                table: "notifications",
                newName: "IX_notifications_notification_event_type");
        }
    }
}
