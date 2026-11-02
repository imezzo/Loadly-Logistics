using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropDriverIdAndNoteFromTripBookingRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trip_booking_requests_drivers_driver_id",
                table: "trip_booking_requests");

            migrationBuilder.DropIndex(
                name: "IX_trip_booking_requests_driver_id",
                table: "trip_booking_requests");

            migrationBuilder.DropColumn(
                name: "driver_id",
                table: "trip_booking_requests");

            migrationBuilder.DropColumn(
                name: "note",
                table: "trip_booking_requests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "driver_id",
                table: "trip_booking_requests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "trip_booking_requests",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_requests_driver_id",
                table: "trip_booking_requests",
                column: "driver_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trip_booking_requests_drivers_driver_id",
                table: "trip_booking_requests",
                column: "driver_id",
                principalTable: "drivers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
