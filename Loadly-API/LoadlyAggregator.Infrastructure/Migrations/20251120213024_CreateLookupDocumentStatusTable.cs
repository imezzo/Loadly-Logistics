using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateLookupDocumentStatusTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "lookup_document_status",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_lookup_document_status", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_lookup_document_status_name_ar",
            table: "lookup_document_status",
            column: "name_ar",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_lookup_document_status_name_en",
            table: "lookup_document_status",
            column: "name_en",
            unique: true);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "lookup_document_status");
    }
}
