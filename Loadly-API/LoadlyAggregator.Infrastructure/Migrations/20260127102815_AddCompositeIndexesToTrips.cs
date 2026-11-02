using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddCompositeIndexesToTrips : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Create composite indexes for common query patterns
        migrationBuilder.CreateIndex(
            name: "IX_trips_status_start_date",
            table: "trips",
            columns: new[] { "status", "start_date" });

        migrationBuilder.CreateIndex(
            name: "IX_trips_contract_id_status",
            table: "trips",
            columns: new[] { "contract_id", "status" });

        migrationBuilder.CreateIndex(
            name: "IX_trips_vehicle_id_start_date",
            table: "trips",
            columns: new[] { "vehicle_id", "start_date" });

        migrationBuilder.CreateIndex(
            name: "IX_trips_driver_id_status",
            table: "trips",
            columns: new[] { "driver_id", "status" });

        migrationBuilder.CreateIndex(
            name: "IX_trips_status_created_at",
            table: "trips",
            columns: new[] { "status", "created_at" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Drop composite indexes
        migrationBuilder.DropIndex(
            name: "IX_trips_status_start_date",
            table: "trips");

        migrationBuilder.DropIndex(
            name: "IX_trips_contract_id_status",
            table: "trips");

        migrationBuilder.DropIndex(
            name: "IX_trips_vehicle_id_start_date",
            table: "trips");

        migrationBuilder.DropIndex(
            name: "IX_trips_driver_id_status",
            table: "trips");

        migrationBuilder.DropIndex(
            name: "IX_trips_status_created_at",
            table: "trips");
    }
}
