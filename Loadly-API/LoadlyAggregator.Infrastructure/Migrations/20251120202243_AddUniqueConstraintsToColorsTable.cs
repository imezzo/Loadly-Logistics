using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddUniqueConstraintsToColorsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_colors_color_hash",
            table: "colors");

        migrationBuilder.CreateTable(
            name: "driver_documents",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                document_id = table.Column<long>(type: "bigint", nullable: false),
                driver_id = table.Column<long>(type: "bigint", nullable: false),
                expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_driver_documents", x => x.id);
                table.ForeignKey(
                    name: "FK_driver_documents_documents_document_id",
                    column: x => x.document_id,
                    principalTable: "documents",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_driver_documents_drivers_driver_id",
                    column: x => x.driver_id,
                    principalTable: "drivers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_colors_color_hash",
            table: "colors",
            column: "color_hash",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_colors_name_ar",
            table: "colors",
            column: "name_ar",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_colors_name_en",
            table: "colors",
            column: "name_en",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_driver_documents_document_id",
            table: "driver_documents",
            column: "document_id");

        migrationBuilder.CreateIndex(
            name: "IX_driver_documents_driver_id",
            table: "driver_documents",
            column: "driver_id");

        migrationBuilder.CreateIndex(
            name: "IX_driver_documents_status",
            table: "driver_documents",
            column: "status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "driver_documents");

        migrationBuilder.DropIndex(
            name: "IX_colors_color_hash",
            table: "colors");

        migrationBuilder.DropIndex(
            name: "IX_colors_name_ar",
            table: "colors");

        migrationBuilder.DropIndex(
            name: "IX_colors_name_en",
            table: "colors");

        migrationBuilder.CreateIndex(
            name: "IX_colors_color_hash",
            table: "colors",
            column: "color_hash");
    }
}
