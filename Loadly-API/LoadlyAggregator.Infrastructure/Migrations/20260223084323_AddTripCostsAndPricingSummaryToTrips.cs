using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddTripCostsAndPricingSummaryToTrips : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "daily_value_cost",
            table: "trips",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "extra_km_cost",
            table: "trips",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "extra_trip_cost",
            table: "trips",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "fuel_cost",
            table: "trips",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "is_extra_trip",
            table: "trips",
            type: "boolean",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "over_capacity_cost",
            table: "trips",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "total_trip_cost",
            table: "trips",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: true);

        migrationBuilder.CreateTable(
            name: "trip_costs",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                trip_id = table.Column<long>(type: "bigint", nullable: false),
                charge_type = table.Column<int>(type: "integer", nullable: false),
                amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                extra_fee_type_id = table.Column<int>(type: "integer", nullable: true),
                notes = table.Column<string>(type: "text", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_trip_costs", x => x.id);
                table.ForeignKey(
                    name: "FK_trip_costs_trips_trip_id",
                    column: x => x.trip_id,
                    principalTable: "trips",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_trip_costs_trip_id",
            table: "trip_costs",
            column: "trip_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "trip_costs");

        migrationBuilder.DropColumn(
            name: "daily_value_cost",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "extra_km_cost",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "extra_trip_cost",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "fuel_cost",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "is_extra_trip",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "over_capacity_cost",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "total_trip_cost",
            table: "trips");
    }
}
