using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIntEnumColumnsToNotificationPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "channel",
                table: "notification_preferences",
                type: "integer",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "event_type",
                table: "notification_preferences",
                type: "integer",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "idx_notification_preferences_model_channel_event",
                table: "notification_preferences",
                columns: new[] { "model_id", "model_type", "channel", "event_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_notification_preferences_model_channel_event",
                table: "notification_preferences");

            migrationBuilder.DropColumn(name: "channel", table: "notification_preferences");
            migrationBuilder.DropColumn(name: "event_type", table: "notification_preferences");
        }
    }
}
