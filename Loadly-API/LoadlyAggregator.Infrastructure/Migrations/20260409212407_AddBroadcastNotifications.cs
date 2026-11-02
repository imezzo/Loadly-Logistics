using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBroadcastNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "broadcast_notifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    added_by = table.Column<long>(type: "bigint", nullable: false),
                    channel = table.Column<int>(type: "integer", nullable: false),
                    model_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    model_ids = table.Column<string>(type: "text", nullable: true),
                    subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    body = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_broadcast_notifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "broadcast_notification_attachments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    broadcast_notification_id = table.Column<long>(type: "bigint", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_broadcast_notification_attachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_broadcast_notification_attachments_broadcast_notifications_~",
                        column: x => x.broadcast_notification_id,
                        principalTable: "broadcast_notifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_broadcast_notification_attachments_broadcast_notification_id",
                table: "broadcast_notification_attachments",
                column: "broadcast_notification_id");

            migrationBuilder.CreateIndex(
                name: "IX_broadcast_notifications_added_by",
                table: "broadcast_notifications",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "IX_broadcast_notifications_channel",
                table: "broadcast_notifications",
                column: "channel");
                
            migrationBuilder.CreateIndex(
                name: "IX_broadcast_notifications_created_at",
                table: "broadcast_notifications",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_broadcast_notifications_model_type",
                table: "broadcast_notifications",
                column: "model_type");

            migrationBuilder.CreateIndex(
                name: "IX_broadcast_notifications_status",
                table: "broadcast_notifications",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "broadcast_notification_attachments");

            migrationBuilder.DropTable(
                name: "broadcast_notifications");
        }
    }
}
