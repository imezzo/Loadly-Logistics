using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class DropRequestedVehiclesAddRequestedVehicleIds : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "requested_vehicles",
            table: "vehicle_change_requests");

        migrationBuilder.AddColumn<long[]>(
            name: "requested_vehicles_ids",
            table: "vehicle_change_requests",
            type: "bigint[]",
            nullable: true);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "requested_vehicles_ids",
            table: "vehicle_change_requests");

        migrationBuilder.AddColumn<string>(
            name: "requested_vehicles",
            table: "vehicle_change_requests",
            type: "text",
            nullable: true);
    }
}
