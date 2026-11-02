using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripDispatchingModeAndBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "dispatching_mode",
                table: "trips",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "trip_booking_requests",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    trip_id = table.Column<long>(type: "bigint", nullable: false),
                    contractor_id = table.Column<long>(type: "bigint", nullable: false),
                    driver_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    note = table.Column<string>(type: "text", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reviewed_by_admin_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_booking_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_booking_requests_admins_reviewed_by_admin_id",
                        column: x => x.reviewed_by_admin_id,
                        principalTable: "admins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_trip_booking_requests_contractors_contractor_id",
                        column: x => x.contractor_id,
                        principalTable: "contractors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trip_booking_requests_drivers_driver_id",
                        column: x => x.driver_id,
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trip_booking_requests_trips_trip_id",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trip_invites",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    trip_id = table.Column<long>(type: "bigint", nullable: false),
                    contractor_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_invites", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_invites_contractors_contractor_id",
                        column: x => x.contractor_id,
                        principalTable: "contractors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trip_invites_trips_trip_id",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_requests_contractor_id",
                table: "trip_booking_requests",
                column: "contractor_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_requests_driver_id",
                table: "trip_booking_requests",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_requests_reviewed_by_admin_id",
                table: "trip_booking_requests",
                column: "reviewed_by_admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_requests_status",
                table: "trip_booking_requests",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_requests_trip_id",
                table: "trip_booking_requests",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_booking_requests_trip_id_contractor_id",
                table: "trip_booking_requests",
                columns: new[] { "trip_id", "contractor_id" },
                unique: true,
                filter: "\"deleted_at\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_trip_invites_contractor_id",
                table: "trip_invites",
                column: "contractor_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_invites_trip_id",
                table: "trip_invites",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_invites_trip_id_contractor_id",
                table: "trip_invites",
                columns: new[] { "trip_id", "contractor_id" },
                unique: true,
                filter: "\"deleted_at\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trip_booking_requests");

            migrationBuilder.DropTable(
                name: "trip_invites");

            migrationBuilder.DropColumn(
                name: "dispatching_mode",
                table: "trips");
        }
    }
}
