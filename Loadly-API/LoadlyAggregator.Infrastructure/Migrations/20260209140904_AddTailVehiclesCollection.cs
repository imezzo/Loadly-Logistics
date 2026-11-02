using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddTailVehiclesCollection : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_vehicles_head_vehicle_id",
            table: "vehicles");

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_head_vehicle_id",
            table: "vehicles",
            column: "head_vehicle_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_vehicles_head_vehicle_id",
            table: "vehicles");

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_head_vehicle_id",
            table: "vehicles",
            column: "head_vehicle_id",
            unique: true,
            filter: "\"head_vehicle_id\" IS NOT NULL");
    }
}
