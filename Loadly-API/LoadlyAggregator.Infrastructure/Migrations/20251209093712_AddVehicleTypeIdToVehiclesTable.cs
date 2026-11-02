using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddVehicleTypeIdToVehiclesTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "vehicle_type_id",
            table: "vehicles",
            type: "bigint",
            nullable: false,
            defaultValue: 0L);

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_vehicle_type_id",
            table: "vehicles",
            column: "vehicle_type_id");

        migrationBuilder.AddForeignKey(
            name: "FK_vehicles_vehicle_types_vehicle_type_id",
            table: "vehicles",
            column: "vehicle_type_id",
            principalTable: "vehicle_types",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_vehicles_vehicle_types_vehicle_type_id",
            table: "vehicles");

        migrationBuilder.DropIndex(
            name: "IX_vehicles_vehicle_type_id",
            table: "vehicles");

        migrationBuilder.DropColumn(
            name: "vehicle_type_id",
            table: "vehicles");
    }
}
