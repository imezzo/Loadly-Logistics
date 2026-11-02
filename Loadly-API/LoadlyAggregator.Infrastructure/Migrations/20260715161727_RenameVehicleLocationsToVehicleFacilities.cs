using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RenameVehicleLocationsToVehicleFacilities : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_vehicles_vehicle_locations_parked_vehicle_location_id",
            table: "vehicles");

        migrationBuilder.DropForeignKey(
            name: "FK_vehicle_location_visits_vehicle_locations_vehicle_location_~",
            table: "vehicle_location_visits");

        migrationBuilder.DropForeignKey(
            name: "FK_vehicle_location_visits_admins_added_by",
            table: "vehicle_location_visits");

        migrationBuilder.DropForeignKey(
            name: "FK_vehicle_location_visits_drivers_driver_id",
            table: "vehicle_location_visits");

        migrationBuilder.DropForeignKey(
            name: "FK_vehicle_location_visits_vehicles_vehicle_id",
            table: "vehicle_location_visits");

        // Polymorphic contacts use model_type + model_id; never add FK on model_id alone
        // (same id can refer to Company, Driver, VehicleFacility, etc.).

        migrationBuilder.RenameTable(
            name: "vehicle_locations",
            newName: "vehicle_facilities");

        migrationBuilder.RenameColumn(
            name: "location_type",
            table: "vehicle_facilities",
            newName: "facility_type");

        migrationBuilder.RenameIndex(
            name: "IX_vehicle_locations_location_type",
            table: "vehicle_facilities",
            newName: "IX_vehicle_facilities_facility_type");

        migrationBuilder.RenameIndex(
            name: "IX_vehicle_locations_status",
            table: "vehicle_facilities",
            newName: "IX_vehicle_facilities_status");

        migrationBuilder.Sql("""
            ALTER INDEX "PK_vehicle_locations" RENAME TO "PK_vehicle_facilities";
            """);

        migrationBuilder.RenameTable(
            name: "vehicle_location_visits",
            newName: "vehicle_facility_visits");

        migrationBuilder.RenameColumn(
            name: "vehicle_location_id",
            table: "vehicle_facility_visits",
            newName: "vehicle_facility_id");

        migrationBuilder.RenameIndex(
            name: "IX_vehicle_location_visits_vehicle_location_id_event_date",
            table: "vehicle_facility_visits",
            newName: "IX_vehicle_facility_visits_vehicle_facility_id_event_date");

        migrationBuilder.RenameIndex(
            name: "IX_vehicle_location_visits_added_by",
            table: "vehicle_facility_visits",
            newName: "IX_vehicle_facility_visits_added_by");

        migrationBuilder.RenameIndex(
            name: "IX_vehicle_location_visits_driver_id",
            table: "vehicle_facility_visits",
            newName: "IX_vehicle_facility_visits_driver_id");

        migrationBuilder.RenameIndex(
            name: "IX_vehicle_location_visits_vehicle_id_event_date",
            table: "vehicle_facility_visits",
            newName: "IX_vehicle_facility_visits_vehicle_id_event_date");

        migrationBuilder.Sql("""
            ALTER INDEX "PK_vehicle_location_visits" RENAME TO "PK_vehicle_facility_visits";
            """);

        migrationBuilder.RenameColumn(
            name: "parked_vehicle_location_id",
            table: "vehicles",
            newName: "parked_vehicle_facility_id");

        migrationBuilder.RenameIndex(
            name: "IX_vehicles_parked_vehicle_location_id",
            table: "vehicles",
            newName: "IX_vehicles_parked_vehicle_facility_id");

        migrationBuilder.AddForeignKey(
            name: "FK_vehicle_facility_visits_admins_added_by",
            table: "vehicle_facility_visits",
            column: "added_by",
            principalTable: "admins",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_vehicle_facility_visits_drivers_driver_id",
            table: "vehicle_facility_visits",
            column: "driver_id",
            principalTable: "drivers",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_vehicle_facility_visits_vehicle_facilities_vehicle_facility~",
            table: "vehicle_facility_visits",
            column: "vehicle_facility_id",
            principalTable: "vehicle_facilities",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_vehicle_facility_visits_vehicles_vehicle_id",
            table: "vehicle_facility_visits",
            column: "vehicle_id",
            principalTable: "vehicles",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_vehicles_vehicle_facilities_parked_vehicle_facility_id",
            table: "vehicles",
            column: "parked_vehicle_facility_id",
            principalTable: "vehicle_facilities",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_vehicles_vehicle_facilities_parked_vehicle_facility_id",
            table: "vehicles");

        migrationBuilder.DropForeignKey(
            name: "FK_vehicle_facility_visits_vehicle_facilities_vehicle_facility~",
            table: "vehicle_facility_visits");

        migrationBuilder.DropForeignKey(
            name: "FK_vehicle_facility_visits_admins_added_by",
            table: "vehicle_facility_visits");

        migrationBuilder.DropForeignKey(
            name: "FK_vehicle_facility_visits_drivers_driver_id",
            table: "vehicle_facility_visits");

        migrationBuilder.DropForeignKey(
            name: "FK_vehicle_facility_visits_vehicles_vehicle_id",
            table: "vehicle_facility_visits");

        migrationBuilder.RenameColumn(
            name: "parked_vehicle_facility_id",
            table: "vehicles",
            newName: "parked_vehicle_location_id");

        migrationBuilder.RenameIndex(
            name: "IX_vehicles_parked_vehicle_facility_id",
            table: "vehicles",
            newName: "IX_vehicles_parked_vehicle_location_id");

        migrationBuilder.RenameColumn(
            name: "vehicle_facility_id",
            table: "vehicle_facility_visits",
            newName: "vehicle_location_id");

        migrationBuilder.RenameIndex(
            name: "IX_vehicle_facility_visits_vehicle_facility_id_event_date",
            table: "vehicle_facility_visits",
            newName: "IX_vehicle_location_visits_vehicle_location_id_event_date");

        migrationBuilder.RenameIndex(
            name: "IX_vehicle_facility_visits_added_by",
            table: "vehicle_facility_visits",
            newName: "IX_vehicle_location_visits_added_by");

        migrationBuilder.RenameIndex(
            name: "IX_vehicle_facility_visits_driver_id",
            table: "vehicle_facility_visits",
            newName: "IX_vehicle_location_visits_driver_id");

        migrationBuilder.RenameIndex(
            name: "IX_vehicle_facility_visits_vehicle_id_event_date",
            table: "vehicle_facility_visits",
            newName: "IX_vehicle_location_visits_vehicle_id_event_date");

        migrationBuilder.Sql("""
            ALTER INDEX "PK_vehicle_facility_visits" RENAME TO "PK_vehicle_location_visits";
            """);

        migrationBuilder.RenameTable(
            name: "vehicle_facility_visits",
            newName: "vehicle_location_visits");

        migrationBuilder.RenameColumn(
            name: "facility_type",
            table: "vehicle_facilities",
            newName: "location_type");

        migrationBuilder.RenameIndex(
            name: "IX_vehicle_facilities_facility_type",
            table: "vehicle_facilities",
            newName: "IX_vehicle_locations_location_type");

        migrationBuilder.RenameIndex(
            name: "IX_vehicle_facilities_status",
            table: "vehicle_facilities",
            newName: "IX_vehicle_locations_status");

        migrationBuilder.Sql("""
            ALTER INDEX "PK_vehicle_facilities" RENAME TO "PK_vehicle_locations";
            """);

        migrationBuilder.RenameTable(
            name: "vehicle_facilities",
            newName: "vehicle_locations");

        migrationBuilder.AddForeignKey(
            name: "FK_vehicle_location_visits_admins_added_by",
            table: "vehicle_location_visits",
            column: "added_by",
            principalTable: "admins",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_vehicle_location_visits_drivers_driver_id",
            table: "vehicle_location_visits",
            column: "driver_id",
            principalTable: "drivers",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_vehicle_location_visits_vehicle_locations_vehicle_location_~",
            table: "vehicle_location_visits",
            column: "vehicle_location_id",
            principalTable: "vehicle_locations",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_vehicle_location_visits_vehicles_vehicle_id",
            table: "vehicle_location_visits",
            column: "vehicle_id",
            principalTable: "vehicles",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_vehicles_vehicle_locations_parked_vehicle_location_id",
            table: "vehicles",
            column: "parked_vehicle_location_id",
            principalTable: "vehicle_locations",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);
    }
}
