using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class MakeDriverLocationSingleRowUpsert : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Keep only the most recent location per driver before enforcing uniqueness
        migrationBuilder.Sql(@"
                DELETE FROM driver_locations
                WHERE id NOT IN (
                    SELECT MAX(id) FROM driver_locations GROUP BY driver_id
                );
            ");

        migrationBuilder.DropIndex(
            name: "IX_driver_locations_driver_id",
            table: "driver_locations");

        migrationBuilder.DropIndex(
            name: "IX_driver_locations_recorded_at",
            table: "driver_locations");

        migrationBuilder.CreateIndex(
            name: "IX_driver_locations_driver_id",
            table: "driver_locations",
            column: "driver_id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_driver_locations_driver_id",
            table: "driver_locations");

        migrationBuilder.CreateIndex(
            name: "IX_driver_locations_driver_id",
            table: "driver_locations",
            column: "driver_id");

        migrationBuilder.CreateIndex(
            name: "IX_driver_locations_recorded_at",
            table: "driver_locations",
            column: "recorded_at");
    }
}
