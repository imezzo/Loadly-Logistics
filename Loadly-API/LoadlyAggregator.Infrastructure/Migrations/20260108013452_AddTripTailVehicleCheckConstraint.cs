using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddTripTailVehicleCheckConstraint : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddCheckConstraint(
            name: "CK_Trip_TailNotSameAsHead",
            table: "trips",
            sql: "\"tail_vehicle_id\" IS NULL OR \"tail_vehicle_id\" != \"vehicle_id\"");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCheckConstraint(
            name: "CK_Trip_TailNotSameAsHead",
            table: "trips");
    }
}
