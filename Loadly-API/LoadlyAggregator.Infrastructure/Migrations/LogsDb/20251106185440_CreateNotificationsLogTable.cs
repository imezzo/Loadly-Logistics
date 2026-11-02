using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations.LogsDb;


public partial class CreateNotificationsLogTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "notifications_log",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                type = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                recipient = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                response = table.Column<string>(type: "text", nullable: true),
                body = table.Column<string>(type: "text", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_notifications_log", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_notifications_log_created_at",
            table: "notifications_log",
            column: "created_at");

        migrationBuilder.CreateIndex(
            name: "IX_notifications_log_recipient",
            table: "notifications_log",
            column: "recipient");

        migrationBuilder.CreateIndex(
            name: "IX_notifications_log_status",
            table: "notifications_log",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "IX_notifications_log_type",
            table: "notifications_log",
            column: "type");

        migrationBuilder.CreateIndex(
            name: "IX_notifications_log_type_status_created_at",
            table: "notifications_log",
            columns: new[] { "type", "status", "created_at" });
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "notifications_log");
    }
}
