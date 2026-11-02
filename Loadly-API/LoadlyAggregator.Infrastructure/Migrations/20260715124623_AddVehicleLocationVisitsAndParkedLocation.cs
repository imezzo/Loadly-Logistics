using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleLocationVisitsAndParkedLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "parked_vehicle_location_id",
                table: "vehicles",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "vehicle_location_visits",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vehicle_id = table.Column<long>(type: "bigint", nullable: false),
                    vehicle_location_id = table.Column<long>(type: "bigint", nullable: false),
                    @event = table.Column<int>(name: "event", type: "integer", nullable: false),
                    event_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    driver_id = table.Column<long>(type: "bigint", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    added_by = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle_location_visits", x => x.id);
                    table.ForeignKey(
                        name: "FK_vehicle_location_visits_admins_added_by",
                        column: x => x.added_by,
                        principalTable: "admins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vehicle_location_visits_drivers_driver_id",
                        column: x => x.driver_id,
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_vehicle_location_visits_vehicle_locations_vehicle_location_~",
                        column: x => x.vehicle_location_id,
                        principalTable: "vehicle_locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vehicle_location_visits_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_parked_vehicle_location_id",
                table: "vehicles",
                column: "parked_vehicle_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_location_visits_added_by",
                table: "vehicle_location_visits",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_location_visits_driver_id",
                table: "vehicle_location_visits",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_location_visits_vehicle_id_event_date",
                table: "vehicle_location_visits",
                columns: new[] { "vehicle_id", "event_date" });

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_location_visits_vehicle_location_id_event_date",
                table: "vehicle_location_visits",
                columns: new[] { "vehicle_location_id", "event_date" });

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_vehicle_locations_parked_vehicle_location_id",
                table: "vehicles",
                column: "parked_vehicle_location_id",
                principalTable: "vehicle_locations",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_vehicle_locations_parked_vehicle_location_id",
                table: "vehicles");

            migrationBuilder.DropTable(
                name: "vehicle_location_visits");

            migrationBuilder.DropIndex(
                name: "IX_vehicles_parked_vehicle_location_id",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "parked_vehicle_location_id",
                table: "vehicles");
        }
    }
}
