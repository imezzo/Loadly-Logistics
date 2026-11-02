using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations.LogsDb;


public partial class CreateAuditLogsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "audit_logs",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                event_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                entity_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                entity_id = table.Column<long>(type: "bigint", nullable: true),
                user_id = table.Column<long>(type: "bigint", nullable: false),
                user_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                changes = table.Column<string>(type: "text", nullable: false),
                ip_address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                additional_data = table.Column<string>(type: "text", nullable: true),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_audit_logs", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_audit_logs_user_id_user_type",
            table: "audit_logs",
            columns: new[] { "user_id", "user_type" });

        migrationBuilder.CreateIndex(
            name: "IX_audit_logs_entity_type_entity_id",
            table: "audit_logs",
            columns: new[] { "entity_type", "entity_id" });

        migrationBuilder.CreateIndex(
            name: "IX_audit_logs_created_at",
            table: "audit_logs",
            column: "created_at");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "audit_logs");
    }
}
