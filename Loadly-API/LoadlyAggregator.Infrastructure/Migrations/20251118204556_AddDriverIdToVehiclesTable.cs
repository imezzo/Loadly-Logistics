using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddDriverIdToVehiclesTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "driver_id",
            table: "vehicles",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_driver_id",
            table: "vehicles",
            column: "driver_id");

        migrationBuilder.AddForeignKey(
            name: "FK_vehicles_drivers_driver_id",
            table: "vehicles",
            column: "driver_id",
            principalTable: "drivers",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_vehicles_drivers_driver_id",
            table: "vehicles");

        migrationBuilder.DropIndex(
            name: "IX_vehicles_driver_id",
            table: "vehicles");

        migrationBuilder.DropColumn(
            name: "driver_id",
            table: "vehicles");
    }
}
