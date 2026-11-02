using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class CreateTripWaypointsTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "trip_waypoints",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                trip_id = table.Column<long>(type: "bigint", nullable: false),
                sequence = table.Column<int>(type: "integer", nullable: false),
                stop_type = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                google_place_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                latitude = table.Column<decimal>(type: "numeric(10,8)", nullable: false),
                longitude = table.Column<decimal>(type: "numeric(11,8)", nullable: false),
                address_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                task_details = table.Column<string>(type: "text", nullable: true),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_trip_waypoints", x => x.id);
                table.ForeignKey(
                    name: "FK_trip_waypoints_trips_trip_id",
                    column: x => x.trip_id,
                    principalTable: "trips",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_trip_waypoints_trip_id",
            table: "trip_waypoints",
            column: "trip_id");

        migrationBuilder.CreateIndex(
            name: "IX_trip_waypoints_trip_id_sequence",
            table: "trip_waypoints",
            columns: new[] { "trip_id", "sequence" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "trip_waypoints");
    }
}
