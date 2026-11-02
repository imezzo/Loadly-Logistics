using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class DropTripDetailsTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "trip_details");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "trip_details",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                trip_id = table.Column<long>(type: "bigint", nullable: false),
                actual_end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                actual_latitude = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                actual_longitude = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                actual_start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                end_odometer_reading = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                start_odometer_reading = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_trip_details", x => x.id);
                table.ForeignKey(
                    name: "FK_trip_details_trips_trip_id",
                    column: x => x.trip_id,
                    principalTable: "trips",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_trip_details_deleted_at",
            table: "trip_details",
            column: "deleted_at");

        migrationBuilder.CreateIndex(
            name: "ix_trip_details_trip_id",
            table: "trip_details",
            column: "trip_id",
            unique: true);
    }
}
