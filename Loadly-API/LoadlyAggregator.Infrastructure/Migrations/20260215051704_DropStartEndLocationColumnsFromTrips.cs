using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class DropStartEndLocationColumnsFromTrips : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_trips_locations_end_location_id",
            table: "trips");

        migrationBuilder.DropForeignKey(
            name: "FK_trips_locations_start_location_id",
            table: "trips");

        migrationBuilder.DropIndex(
            name: "IX_vehicle_details_vehicle_id",
            table: "vehicle_details");

        migrationBuilder.DropIndex(
            name: "IX_trips_end_location_id",
            table: "trips");

        migrationBuilder.DropIndex(
            name: "IX_trips_start_location_id",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "end_location_id",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "start_location_id",
            table: "trips");

        migrationBuilder.CreateIndex(
            name: "IX_vehicle_details_vehicle_id",
            table: "vehicle_details",
            column: "vehicle_id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_vehicle_details_vehicle_id",
            table: "vehicle_details");

        migrationBuilder.AddColumn<long>(
            name: "end_location_id",
            table: "trips",
            type: "bigint",
            nullable: true);

        migrationBuilder.AddColumn<long>(
            name: "start_location_id",
            table: "trips",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_vehicle_details_vehicle_id",
            table: "vehicle_details",
            column: "vehicle_id");

        migrationBuilder.CreateIndex(
            name: "IX_trips_end_location_id",
            table: "trips",
            column: "end_location_id");

        migrationBuilder.CreateIndex(
            name: "IX_trips_start_location_id",
            table: "trips",
            column: "start_location_id");

        migrationBuilder.AddForeignKey(
            name: "FK_trips_locations_end_location_id",
            table: "trips",
            column: "end_location_id",
            principalTable: "locations",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_trips_locations_start_location_id",
            table: "trips",
            column: "start_location_id",
            principalTable: "locations",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);
    }
}
