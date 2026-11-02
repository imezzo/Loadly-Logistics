using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class DropColumnsFromTripsTable : Migration
{

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Note: distance column is NOT dropped here - it should remain in the trips table
        // The distance column was added in 20251222142542_AddDistanceToTrips and should persist

        migrationBuilder.DropColumn(
            name: "end_location",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "fuel_quantity",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "start_location",
            table: "trips");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Note: distance column is NOT re-added here in Down() since it wasn't dropped in Up()

        migrationBuilder.AddColumn<string>(
            name: "end_location",
            table: "trips",
            type: "character varying(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<decimal>(
            name: "fuel_quantity",
            table: "trips",
            type: "numeric(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "start_location",
            table: "trips",
            type: "character varying(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "");
    }
}
