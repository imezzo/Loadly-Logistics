using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class CreateDeviceTokensTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "device_tokens",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                user_id = table.Column<long>(type: "bigint", nullable: false),
                user_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                fcm_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                device_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                device_model = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                device_os = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                app_version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                last_used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_device_tokens", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_device_tokens_fcm_token",
            table: "device_tokens",
            column: "fcm_token");

        migrationBuilder.CreateIndex(
            name: "IX_device_tokens_is_active",
            table: "device_tokens",
            column: "is_active",
            filter: "\"is_active\" = true");

        migrationBuilder.CreateIndex(
            name: "IX_device_tokens_user_id_user_type",
            table: "device_tokens",
            columns: new[] { "user_id", "user_type" });

        migrationBuilder.CreateIndex(
            name: "IX_device_tokens_user_type_token_unique",
            table: "device_tokens",
            columns: new[] { "user_id", "user_type", "fcm_token" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "device_tokens");
    }
}
