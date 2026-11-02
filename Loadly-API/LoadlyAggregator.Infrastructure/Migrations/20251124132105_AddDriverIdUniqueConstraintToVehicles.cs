using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddDriverIdUniqueConstraintToVehicles : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_vehicles_driver_id",
            table: "vehicles");

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_driver_id",
            table: "vehicles",
            column: "driver_id",
            unique: true);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_vehicles_driver_id",
            table: "vehicles");

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_driver_id",
            table: "vehicles",
            column: "driver_id");
    }
}
