using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractorIdToVehicleAndDriverAndTripTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "contractor_id",
                table: "vehicles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "contractor_id",
                table: "trips",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "contractor_id",
                table: "drivers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_contractor_id",
                table: "vehicles",
                column: "contractor_id");

            migrationBuilder.CreateIndex(
                name: "IX_trips_contractor_id",
                table: "trips",
                column: "contractor_id");

            migrationBuilder.CreateIndex(
                name: "IX_drivers_contractor_id",
                table: "drivers",
                column: "contractor_id");

            migrationBuilder.AddForeignKey(
                name: "FK_drivers_contractors_contractor_id",
                table: "drivers",
                column: "contractor_id",
                principalTable: "contractors",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_trips_contractors_contractor_id",
                table: "trips",
                column: "contractor_id",
                principalTable: "contractors",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_contractors_contractor_id",
                table: "vehicles",
                column: "contractor_id",
                principalTable: "contractors",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_drivers_contractors_contractor_id",
                table: "drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_trips_contractors_contractor_id",
                table: "trips");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_contractors_contractor_id",
                table: "vehicles");

            migrationBuilder.DropIndex(
                name: "IX_vehicles_contractor_id",
                table: "vehicles");

            migrationBuilder.DropIndex(
                name: "IX_trips_contractor_id",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_drivers_contractor_id",
                table: "drivers");

            migrationBuilder.DropColumn(
                name: "contractor_id",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "contractor_id",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "contractor_id",
                table: "drivers");
        }
    }
}
