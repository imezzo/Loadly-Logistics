using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddVehicleBrandsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "vehicle_brands",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                logo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_vehicle_brands", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_vehicle_brands_status",
            table: "vehicle_brands",
            column: "status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "vehicle_brands");
    }
}
