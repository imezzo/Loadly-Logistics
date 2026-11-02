using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAddedByToTripBookingInvitationAndRound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "added_by",
                table: "trip_booking_invitations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "added_by",
                table: "trip_booking_invitation_rounds",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "added_by",
                table: "trip_booking_invitations");

            migrationBuilder.DropColumn(
                name: "added_by",
                table: "trip_booking_invitation_rounds");
        }
    }
}
