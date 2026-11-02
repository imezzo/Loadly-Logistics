using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RenameTripInvitesToTripBookingInvitations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_trip_invites_contractors_contractor_id",
            table: "trip_invites");

        migrationBuilder.DropForeignKey(
            name: "FK_trip_invites_trip_booking_invitation_rounds_trip_booking_in~",
            table: "trip_invites");

        migrationBuilder.DropForeignKey(
            name: "FK_trip_invites_trips_trip_id",
            table: "trip_invites");

        migrationBuilder.DropPrimaryKey(
            name: "PK_trip_invites",
            table: "trip_invites");

        migrationBuilder.RenameTable(
            name: "trip_invites",
            newName: "trip_booking_invitations");

        migrationBuilder.RenameIndex(
            name: "IX_trip_invites_contractor_id",
            table: "trip_booking_invitations",
            newName: "IX_trip_booking_invitations_contractor_id");

        migrationBuilder.RenameIndex(
            name: "IX_trip_invites_status",
            table: "trip_booking_invitations",
            newName: "IX_trip_booking_invitations_status");

        migrationBuilder.RenameIndex(
            name: "IX_trip_invites_trip_booking_invitation_round_id",
            table: "trip_booking_invitations",
            newName: "IX_trip_booking_invitations_trip_booking_invitation_round_id");

        migrationBuilder.RenameIndex(
            name: "IX_trip_invites_trip_booking_invitation_round_id_contractor_id",
            table: "trip_booking_invitations",
            newName: "IX_trip_booking_invitations_trip_booking_invitation_round_id_c~");

        migrationBuilder.RenameIndex(
            name: "IX_trip_invites_trip_id",
            table: "trip_booking_invitations",
            newName: "IX_trip_booking_invitations_trip_id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_trip_booking_invitations",
            table: "trip_booking_invitations",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "FK_trip_booking_invitations_contractors_contractor_id",
            table: "trip_booking_invitations",
            column: "contractor_id",
            principalTable: "contractors",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_trip_booking_invitations_trip_booking_invitation_rounds_tri~",
            table: "trip_booking_invitations",
            column: "trip_booking_invitation_round_id",
            principalTable: "trip_booking_invitation_rounds",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_trip_booking_invitations_trips_trip_id",
            table: "trip_booking_invitations",
            column: "trip_id",
            principalTable: "trips",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_trip_booking_invitations_contractors_contractor_id",
            table: "trip_booking_invitations");

        migrationBuilder.DropForeignKey(
            name: "FK_trip_booking_invitations_trip_booking_invitation_rounds_tri~",
            table: "trip_booking_invitations");

        migrationBuilder.DropForeignKey(
            name: "FK_trip_booking_invitations_trips_trip_id",
            table: "trip_booking_invitations");

        migrationBuilder.DropPrimaryKey(
            name: "PK_trip_booking_invitations",
            table: "trip_booking_invitations");

        migrationBuilder.RenameTable(
            name: "trip_booking_invitations",
            newName: "trip_invites");

        migrationBuilder.RenameIndex(
            name: "IX_trip_booking_invitations_contractor_id",
            table: "trip_invites",
            newName: "IX_trip_invites_contractor_id");

        migrationBuilder.RenameIndex(
            name: "IX_trip_booking_invitations_status",
            table: "trip_invites",
            newName: "IX_trip_invites_status");

        migrationBuilder.RenameIndex(
            name: "IX_trip_booking_invitations_trip_booking_invitation_round_id",
            table: "trip_invites",
            newName: "IX_trip_invites_trip_booking_invitation_round_id");

        migrationBuilder.RenameIndex(
            name: "IX_trip_booking_invitations_trip_booking_invitation_round_id_c~",
            table: "trip_invites",
            newName: "IX_trip_invites_trip_booking_invitation_round_id_contractor_id");

        migrationBuilder.RenameIndex(
            name: "IX_trip_booking_invitations_trip_id",
            table: "trip_invites",
            newName: "IX_trip_invites_trip_id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_trip_invites",
            table: "trip_invites",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "FK_trip_invites_contractors_contractor_id",
            table: "trip_invites",
            column: "contractor_id",
            principalTable: "contractors",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_trip_invites_trip_booking_invitation_rounds_trip_booking_in~",
            table: "trip_invites",
            column: "trip_booking_invitation_round_id",
            principalTable: "trip_booking_invitation_rounds",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_trip_invites_trips_trip_id",
            table: "trip_invites",
            column: "trip_id",
            principalTable: "trips",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
