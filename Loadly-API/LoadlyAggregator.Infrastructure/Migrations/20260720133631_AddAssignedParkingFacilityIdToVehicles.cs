using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedParkingFacilityIdToVehicles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "assigned_parking_facility_id",
                table: "vehicles",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_assigned_parking_facility_id",
                table: "vehicles",
                column: "assigned_parking_facility_id");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_vehicle_facilities_assigned_parking_facility_id",
                table: "vehicles",
                column: "assigned_parking_facility_id",
                principalTable: "vehicle_facilities",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_vehicle_facilities_assigned_parking_facility_id",
                table: "vehicles");

            migrationBuilder.DropIndex(
                name: "IX_vehicles_assigned_parking_facility_id",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "assigned_parking_facility_id",
                table: "vehicles");
        }
    }
}
