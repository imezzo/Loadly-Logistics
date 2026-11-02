using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations.LogsDb
{
    /// <inheritdoc />
    public partial class AddModelIdAndModelTypeToNotificationsLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "model_id",
                table: "notifications_log",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "model_type",
                table: "notifications_log",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "recipient_type",
                table: "notifications_log",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_log_model_type_model_id",
                table: "notifications_log",
                columns: new[] { "model_type", "model_id" });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_log_recipient_type",
                table: "notifications_log",
                column: "recipient_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_log_model_type_model_id",
                table: "notifications_log");

            migrationBuilder.DropIndex(
                name: "IX_notifications_log_recipient_type",
                table: "notifications_log");

            migrationBuilder.DropColumn(
                name: "model_id",
                table: "notifications_log");

            migrationBuilder.DropColumn(
                name: "model_type",
                table: "notifications_log");

            migrationBuilder.DropColumn(
                name: "recipient_type",
                table: "notifications_log");
        }
    }
}
