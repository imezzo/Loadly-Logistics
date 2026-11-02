using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddContractVehicleTypeIdToTrips : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "contract_vehicle_type_id",
            table: "trips",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_trips_contract_vehicle_type_id",
            table: "trips",
            column: "contract_vehicle_type_id");

        migrationBuilder.AddForeignKey(
            name: "FK_trips_contract_vehicle_types_contract_vehicle_type_id",
            table: "trips",
            column: "contract_vehicle_type_id",
            principalTable: "contract_vehicle_types",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_trips_contract_vehicle_types_contract_vehicle_type_id",
            table: "trips");

        migrationBuilder.DropIndex(
            name: "IX_trips_contract_vehicle_type_id",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "contract_vehicle_type_id",
            table: "trips");
    }
}
