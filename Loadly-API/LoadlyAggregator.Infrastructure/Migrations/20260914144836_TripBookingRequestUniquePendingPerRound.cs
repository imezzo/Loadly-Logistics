using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TripBookingRequestUniquePendingPerRound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_trip_booking_requests_trip_booking_invitation_round_id_cont~",
                table: "trip_booking_requests");

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_requests_trip_booking_invitation_round_id_cont~",
                table: "trip_booking_requests",
                columns: new[] { "trip_booking_invitation_round_id", "contractor_id" },
                unique: true,
                filter: "\"deleted_at\" IS NULL AND \"trip_booking_invitation_round_id\" IS NOT NULL AND \"status\" = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_trip_booking_requests_trip_booking_invitation_round_id_cont~",
                table: "trip_booking_requests");

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_requests_trip_booking_invitation_round_id_cont~",
                table: "trip_booking_requests",
                columns: new[] { "trip_booking_invitation_round_id", "contractor_id" },
                unique: true,
                filter: "\"deleted_at\" IS NULL AND \"trip_booking_invitation_round_id\" IS NOT NULL");
        }
    }
}
