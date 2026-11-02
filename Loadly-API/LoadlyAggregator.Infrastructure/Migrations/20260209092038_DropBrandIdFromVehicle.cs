using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class DropBrandIdFromVehicle : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_vehicles_vehicle_brands_brand_id",
            table: "vehicles");

        migrationBuilder.DropIndex(
            name: "IX_vehicles_brand_id",
            table: "vehicles");

        migrationBuilder.DropColumn(
            name: "brand_id",
            table: "vehicles");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "brand_id",
            table: "vehicles",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_brand_id",
            table: "vehicles",
            column: "brand_id");

        migrationBuilder.AddForeignKey(
            name: "FK_vehicles_vehicle_brands_brand_id",
            table: "vehicles",
            column: "brand_id",
            principalTable: "vehicle_brands",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);
    }
}
