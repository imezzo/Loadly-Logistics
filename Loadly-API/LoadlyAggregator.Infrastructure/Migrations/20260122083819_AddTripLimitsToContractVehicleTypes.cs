using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddTripLimitsToContractVehicleTypes : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "max_kms",
            table: "contract_vehicle_types",
            type: "numeric(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "trips_per_day",
            table: "contract_vehicle_types",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "trips_per_month",
            table: "contract_vehicle_types",
            type: "integer",
            nullable: true);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "max_kms",
            table: "contract_vehicle_types");

        migrationBuilder.DropColumn(
            name: "trips_per_day",
            table: "contract_vehicle_types");

        migrationBuilder.DropColumn(
            name: "trips_per_month",
            table: "contract_vehicle_types");
    }
}
