using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTypeToEventTypeOnNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "type",
                table: "notifications",
                newName: "event_type");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_type",
                table: "notifications",
                newName: "IX_notifications_event_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "event_type",
                table: "notifications",
                newName: "type");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_event_type",
                table: "notifications",
                newName: "IX_notifications_type");
        }
    }
}
