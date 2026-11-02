using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class DropUpdatedByColumnFromVehicleChangeRequests : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "updated_by",
            table: "vehicle_change_requests");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "updated_by",
            table: "vehicle_change_requests",
            type: "bigint",
            nullable: true);
    }
}
