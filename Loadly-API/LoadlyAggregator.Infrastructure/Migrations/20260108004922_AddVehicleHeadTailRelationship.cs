using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddVehicleHeadTailRelationship : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "is_head",
            table: "vehicles");

        migrationBuilder.AddColumn<long>(
            name: "head_vehicle_id",
            table: "vehicles",
            type: "bigint",
            nullable: true);

        migrationBuilder.AddColumn<long>(
            name: "driver_id",
            table: "trips",
            type: "bigint",
            nullable: true);

        migrationBuilder.AddColumn<long>(
            name: "tail_vehicle_id",
            table: "trips",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_head_vehicle_id",
            table: "vehicles",
            column: "head_vehicle_id",
            unique: true,
            filter: "\"head_vehicle_id\" IS NOT NULL");

        migrationBuilder.AddCheckConstraint(
            name: "CK_Vehicle_NotSelfHead",
            table: "vehicles",
            sql: "\"head_vehicle_id\" IS NULL OR \"head_vehicle_id\" != id");

        migrationBuilder.CreateIndex(
            name: "IX_trips_driver_id",
            table: "trips",
            column: "driver_id");

        migrationBuilder.CreateIndex(
            name: "IX_trips_tail_vehicle_id",
            table: "trips",
            column: "tail_vehicle_id");

        migrationBuilder.AddForeignKey(
            name: "FK_trips_drivers_driver_id",
            table: "trips",
            column: "driver_id",
            principalTable: "drivers",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_trips_vehicles_tail_vehicle_id",
            table: "trips",
            column: "tail_vehicle_id",
            principalTable: "vehicles",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_vehicles_vehicles_head_vehicle_id",
            table: "vehicles",
            column: "head_vehicle_id",
            principalTable: "vehicles",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_trips_drivers_driver_id",
            table: "trips");

        migrationBuilder.DropForeignKey(
            name: "FK_trips_vehicles_tail_vehicle_id",
            table: "trips");

        migrationBuilder.DropForeignKey(
            name: "FK_vehicles_vehicles_head_vehicle_id",
            table: "vehicles");

        migrationBuilder.DropIndex(
            name: "IX_vehicles_head_vehicle_id",
            table: "vehicles");

        migrationBuilder.DropCheckConstraint(
            name: "CK_Vehicle_NotSelfHead",
            table: "vehicles");

        migrationBuilder.DropIndex(
            name: "IX_trips_driver_id",
            table: "trips");

        migrationBuilder.DropIndex(
            name: "IX_trips_tail_vehicle_id",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "head_vehicle_id",
            table: "vehicles");

        migrationBuilder.DropColumn(
            name: "driver_id",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "tail_vehicle_id",
            table: "trips");

        migrationBuilder.AddColumn<bool>(
            name: "is_head",
            table: "vehicles",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }
}
