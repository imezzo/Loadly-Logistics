using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddTripCompletionTracking : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "actual_end_date",
            table: "trips",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "actual_start_date",
            table: "trips",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "end_odometer_reading",
            table: "trips",
            type: "numeric(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "start_odometer_reading",
            table: "trips",
            type: "numeric(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "actual_latitude",
            table: "trips",
            type: "numeric(18,6)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "actual_longitude",
            table: "trips",
            type: "numeric(18,6)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "actual_end_date",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "actual_start_date",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "end_odometer_reading",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "start_odometer_reading",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "actual_latitude",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "actual_longitude",
            table: "trips");
    }
}
