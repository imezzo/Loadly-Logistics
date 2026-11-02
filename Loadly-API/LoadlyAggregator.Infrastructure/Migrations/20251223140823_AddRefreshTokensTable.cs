using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddRefreshTokensTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "refresh_tokens",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                token_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                model_id = table.Column<long>(type: "bigint", nullable: false),
                model_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                token_family_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                device_info = table.Column<string>(type: "text", nullable: true),
                expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                replaced_by_token_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_refresh_tokens", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_refresh_tokens_expires_at",
            table: "refresh_tokens",
            column: "expires_at");

        migrationBuilder.CreateIndex(
            name: "IX_refresh_tokens_model_id_model_type",
            table: "refresh_tokens",
            columns: new[] { "model_id", "model_type" });

        migrationBuilder.CreateIndex(
            name: "IX_refresh_tokens_token_family_id",
            table: "refresh_tokens",
            column: "token_family_id");

        migrationBuilder.CreateIndex(
            name: "IX_refresh_tokens_token_hash",
            table: "refresh_tokens",
            column: "token_hash",
            unique: true);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "refresh_tokens");
    }
}
