using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTailVehilcleIdToTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "tail_vehicle_id",
                table: "trips",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trips_tail_vehicle_id",
                table: "trips",
                column: "tail_vehicle_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_vehicles_tail_vehicle_id",
                table: "trips",
                column: "tail_vehicle_id",
                principalTable: "vehicles",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_vehicles_tail_vehicle_id",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_tail_vehicle_id",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "tail_vehicle_id",
                table: "trips");
        }
    }
}
