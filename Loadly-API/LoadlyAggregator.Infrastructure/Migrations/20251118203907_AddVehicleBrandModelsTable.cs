using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddVehicleBrandModelsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "vehicle_brand_models",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                vehicle_brand_id = table.Column<long>(type: "bigint", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_vehicle_brand_models", x => x.id);
                table.ForeignKey(
                    name: "FK_vehicle_brand_models_vehicle_brands_vehicle_brand_id",
                    column: x => x.vehicle_brand_id,
                    principalTable: "vehicle_brands",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_vehicle_brand_models_status",
            table: "vehicle_brand_models",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "IX_vehicle_brand_models_vehicle_brand_id",
            table: "vehicle_brand_models",
            column: "vehicle_brand_id");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "vehicle_brand_models");
    }
}
