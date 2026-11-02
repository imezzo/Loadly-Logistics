using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameParkedVehicleFacilityToCurrentServiceFacility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_vehicle_facilities_parked_vehicle_facility_id",
                table: "vehicles");

            migrationBuilder.RenameColumn(
                name: "parked_vehicle_facility_id",
                table: "vehicles",
                newName: "current_service_facility_id");

            migrationBuilder.RenameIndex(
                name: "IX_vehicles_parked_vehicle_facility_id",
                table: "vehicles",
                newName: "IX_vehicles_current_service_facility_id");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_vehicle_facilities_current_service_facility_id",
                table: "vehicles",
                column: "current_service_facility_id",
                principalTable: "vehicle_facilities",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_vehicle_facilities_current_service_facility_id",
                table: "vehicles");

            migrationBuilder.RenameColumn(
                name: "current_service_facility_id",
                table: "vehicles",
                newName: "parked_vehicle_facility_id");

            migrationBuilder.RenameIndex(
                name: "IX_vehicles_current_service_facility_id",
                table: "vehicles",
                newName: "IX_vehicles_parked_vehicle_facility_id");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_vehicle_facilities_parked_vehicle_facility_id",
                table: "vehicles",
                column: "parked_vehicle_facility_id",
                principalTable: "vehicle_facilities",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
