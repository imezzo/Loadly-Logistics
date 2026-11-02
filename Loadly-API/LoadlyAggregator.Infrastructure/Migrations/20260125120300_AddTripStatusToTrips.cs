using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddTripStatusToTrips : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "status",
            table: "trips",
            type: "integer",
    nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "IX_trips_status",
            table: "trips",
            column: "status");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_trips_status",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "status",
            table: "trips");
    }
}
