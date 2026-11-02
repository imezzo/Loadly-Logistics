using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropStringColumnsFromNotificationPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_notification_preferences_model_channel_event",
                table: "notification_preferences");

            migrationBuilder.DropColumn(name: "channel", table: "notification_preferences");
            migrationBuilder.DropColumn(name: "event_type", table: "notification_preferences");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "channel",
                table: "notification_preferences",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "event_type",
                table: "notification_preferences",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "idx_notification_preferences_model_channel_event",
                table: "notification_preferences",
                columns: new[] { "model_id", "model_type", "channel", "event_type" },
                unique: true);
        }
    }
}
