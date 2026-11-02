using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicTokenToBroadcastNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "public_token",
                table: "broadcast_notifications",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_broadcast_notifications_public_token",
                table: "broadcast_notifications",
                column: "public_token",
                unique: true,
                filter: "public_token IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_broadcast_notifications_public_token",
                table: "broadcast_notifications");

            migrationBuilder.DropColumn(
                name: "public_token",
                table: "broadcast_notifications");
        }
    }
}
