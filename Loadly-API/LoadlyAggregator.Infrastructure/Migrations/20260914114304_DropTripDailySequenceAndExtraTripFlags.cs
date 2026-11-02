using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropTripDailySequenceAndExtraTripFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "daily_sequence",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "is_daily_extra_trip",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "is_extra_trip",
                table: "trips");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "daily_sequence",
                table: "trips",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_daily_extra_trip",
                table: "trips",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_extra_trip",
                table: "trips",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
