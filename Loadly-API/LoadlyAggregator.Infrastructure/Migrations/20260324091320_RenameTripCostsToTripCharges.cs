using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RenameTripCostsToTripCharges : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_trip_costs_trips_trip_id",
            table: "trip_costs");

        migrationBuilder.DropPrimaryKey(
            name: "PK_trip_costs",
            table: "trip_costs");

        migrationBuilder.RenameTable(
            name: "trip_costs",
            newName: "trip_charges");

        migrationBuilder.RenameIndex(
            name: "IX_trip_costs_trip_id",
            table: "trip_charges",
            newName: "IX_trip_charges_trip_id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_trip_charges",
            table: "trip_charges",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "FK_trip_charges_trips_trip_id",
            table: "trip_charges",
            column: "trip_id",
            principalTable: "trips",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_trip_charges_trips_trip_id",
            table: "trip_charges");

        migrationBuilder.DropPrimaryKey(
            name: "PK_trip_charges",
            table: "trip_charges");

        migrationBuilder.RenameTable(
            name: "trip_charges",
            newName: "trip_costs");

        migrationBuilder.RenameIndex(
            name: "IX_trip_charges_trip_id",
            table: "trip_costs",
            newName: "IX_trip_costs_trip_id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_trip_costs",
            table: "trip_costs",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "FK_trip_costs_trips_trip_id",
            table: "trip_costs",
            column: "trip_id",
            principalTable: "trips",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
