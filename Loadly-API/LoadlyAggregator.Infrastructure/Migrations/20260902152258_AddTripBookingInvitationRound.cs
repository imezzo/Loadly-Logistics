using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripBookingInvitationRound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_trip_invites_trip_id_contractor_id",
                table: "trip_invites");

            migrationBuilder.DropIndex(
                name: "IX_trip_booking_requests_trip_id_contractor_id",
                table: "trip_booking_requests");

            migrationBuilder.AddColumn<long>(
                name: "trip_booking_invitation_round_id",
                table: "trip_invites",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "trip_booking_invitation_round_id",
                table: "trip_booking_requests",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "trip_booking_invitation_rounds",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    trip_id = table.Column<long>(type: "bigint", nullable: false),
                    contractor_id = table.Column<long>(type: "bigint", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_booking_invitation_rounds", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_booking_invitation_rounds_contractors_contractor_id",
                        column: x => x.contractor_id,
                        principalTable: "contractors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trip_booking_invitation_rounds_trips_trip_id",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trip_invites_trip_booking_invitation_round_id",
                table: "trip_invites",
                column: "trip_booking_invitation_round_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_invites_trip_booking_invitation_round_id_contractor_id",
                table: "trip_invites",
                columns: new[] { "trip_booking_invitation_round_id", "contractor_id" },
                unique: true,
                filter: "\"deleted_at\" IS NULL AND \"trip_booking_invitation_round_id\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_requests_trip_booking_invitation_round_id",
                table: "trip_booking_requests",
                column: "trip_booking_invitation_round_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_requests_trip_booking_invitation_round_id_cont~",
                table: "trip_booking_requests",
                columns: new[] { "trip_booking_invitation_round_id", "contractor_id" },
                unique: true,
                filter: "\"deleted_at\" IS NULL AND \"trip_booking_invitation_round_id\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_invitation_rounds_contractor_id",
                table: "trip_booking_invitation_rounds",
                column: "contractor_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_invitation_rounds_status",
                table: "trip_booking_invitation_rounds",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_invitation_rounds_trip_id",
                table: "trip_booking_invitation_rounds",
                column: "trip_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trip_booking_requests_trip_booking_invitation_rounds_trip_b~",
                table: "trip_booking_requests",
                column: "trip_booking_invitation_round_id",
                principalTable: "trip_booking_invitation_rounds",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_trip_invites_trip_booking_invitation_rounds_trip_booking_in~",
                table: "trip_invites",
                column: "trip_booking_invitation_round_id",
                principalTable: "trip_booking_invitation_rounds",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trip_booking_requests_trip_booking_invitation_rounds_trip_b~",
                table: "trip_booking_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_trip_invites_trip_booking_invitation_rounds_trip_booking_in~",
                table: "trip_invites");

            migrationBuilder.DropTable(
                name: "trip_booking_invitation_rounds");

            migrationBuilder.DropIndex(
                name: "IX_trip_invites_trip_booking_invitation_round_id",
                table: "trip_invites");

            migrationBuilder.DropIndex(
                name: "IX_trip_invites_trip_booking_invitation_round_id_contractor_id",
                table: "trip_invites");

            migrationBuilder.DropIndex(
                name: "IX_trip_booking_requests_trip_booking_invitation_round_id",
                table: "trip_booking_requests");

            migrationBuilder.DropIndex(
                name: "IX_trip_booking_requests_trip_booking_invitation_round_id_cont~",
                table: "trip_booking_requests");

            migrationBuilder.DropColumn(
                name: "trip_booking_invitation_round_id",
                table: "trip_invites");

            migrationBuilder.DropColumn(
                name: "trip_booking_invitation_round_id",
                table: "trip_booking_requests");

            migrationBuilder.CreateIndex(
                name: "IX_trip_invites_trip_id_contractor_id",
                table: "trip_invites",
                columns: new[] { "trip_id", "contractor_id" },
                unique: true,
                filter: "\"deleted_at\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_requests_trip_id_contractor_id",
                table: "trip_booking_requests",
                columns: new[] { "trip_id", "contractor_id" },
                unique: true,
                filter: "\"deleted_at\" IS NULL");
        }
    }
}
