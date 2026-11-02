using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddPhotoToVehicleChangeRequest : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Photo",
            table: "vehicle_change_requests",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<long>(
            name: "current_vehicle_id",
            table: "vehicle_change_requests",
            type: "bigint",
            nullable: false,
            defaultValue: 0L);

        migrationBuilder.CreateIndex(
            name: "IX_vehicle_change_requests_current_vehicle_id",
            table: "vehicle_change_requests",
            column: "current_vehicle_id");

        migrationBuilder.AddForeignKey(
            name: "FK_vehicle_change_requests_vehicles_current_vehicle_id",
            table: "vehicle_change_requests",
            column: "current_vehicle_id",
            principalTable: "vehicles",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_vehicle_change_requests_vehicles_current_vehicle_id",
            table: "vehicle_change_requests");

        migrationBuilder.DropIndex(
            name: "IX_vehicle_change_requests_current_vehicle_id",
            table: "vehicle_change_requests");

        migrationBuilder.DropColumn(
            name: "Photo",
            table: "vehicle_change_requests");

        migrationBuilder.DropColumn(
            name: "current_vehicle_id",
            table: "vehicle_change_requests");
    }
}
