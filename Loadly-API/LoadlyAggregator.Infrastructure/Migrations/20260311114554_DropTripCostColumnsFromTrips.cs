using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropTripCostColumnsFromTrips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "over_capacity_cost",
                table: "trips");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<decimal>(
                name: "over_capacity_cost",
                table: "trips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }
    }
}
