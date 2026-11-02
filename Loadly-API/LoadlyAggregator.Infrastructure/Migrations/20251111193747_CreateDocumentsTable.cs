using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateDocumentsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "documents",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                model_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                model_id = table.Column<long>(type: "bigint", nullable: false),
                name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                required = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_documents", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_documents_model_id_model_type",
            table: "documents",
            columns: new[] { "model_id", "model_type" });

        migrationBuilder.CreateIndex(
            name: "IX_documents_status",
            table: "documents",
            column: "status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "documents");
    }
}
