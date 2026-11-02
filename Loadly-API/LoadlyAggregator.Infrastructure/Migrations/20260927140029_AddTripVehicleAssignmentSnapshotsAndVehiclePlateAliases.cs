using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripVehicleAssignmentSnapshotsAndVehiclePlateAliases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trip_vehicle_assignment_snapshot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    trip_id = table.Column<long>(type: "bigint", nullable: false),
                    head_plate_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    head_plate_letters = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    tail_plate_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    tail_plate_letters = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    added_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_vehicle_assignment_snapshot", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_vehicle_assignment_snapshot_admins_added_by",
                        column: x => x.added_by,
                        principalTable: "admins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_trip_vehicle_assignment_snapshot_trips_trip_id",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_plate_aliases",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vehicle_id = table.Column<long>(type: "bigint", nullable: false),
                    plate_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    plate_letters = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    added_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle_plate_aliases", x => x.id);
                    table.ForeignKey(
                        name: "FK_vehicle_plate_aliases_admins_added_by",
                        column: x => x.added_by,
                        principalTable: "admins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_vehicle_plate_aliases_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trip_vehicle_assignment_snapshot_added_by",
                table: "trip_vehicle_assignment_snapshot",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "IX_trip_vehicle_assignment_snapshot_head_plate_no_head_plate_letters",
                table: "trip_vehicle_assignment_snapshot",
                columns: new[] { "head_plate_no", "head_plate_letters" });

            migrationBuilder.CreateIndex(
                name: "IX_trip_vehicle_assignment_snapshot_tail_plate_no_tail_plate_letters",
                table: "trip_vehicle_assignment_snapshot",
                columns: new[] { "tail_plate_no", "tail_plate_letters" });

            migrationBuilder.CreateIndex(
                name: "IX_trip_vehicle_assignment_snapshot_trip_id",
                table: "trip_vehicle_assignment_snapshot",
                column: "trip_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_plate_aliases_plate_no_plate_letters",
                table: "vehicle_plate_aliases",
                columns: new[] { "plate_no", "plate_letters" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_plate_aliases_added_by",
                table: "vehicle_plate_aliases",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_plate_aliases_vehicle_id",
                table: "vehicle_plate_aliases",
                column: "vehicle_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trip_vehicle_assignment_snapshot");

            migrationBuilder.DropTable(
                name: "vehicle_plate_aliases");
        }
    }
}
