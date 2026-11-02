using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ChangeRequestedVehicleToSingle : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "requested_vehicles_ids",
            table: "vehicle_change_requests");

        migrationBuilder.AddColumn<long>(
            name: "requested_vehicle_id",
            table: "vehicle_change_requests",
            type: "bigint",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "requested_vehicle_id",
            table: "vehicle_change_requests");

        migrationBuilder.AddColumn<long[]>(
            name: "requested_vehicles_ids",
            table: "vehicle_change_requests",
            type: "bigint[]",
            nullable: true);
    }
}
