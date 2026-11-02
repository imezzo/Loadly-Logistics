using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropTripsPerMonthTripsPerDayMaxKmsFromContracts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "max_kms",
                table: "contracts");

            migrationBuilder.DropColumn(
                name: "trips_per_day",
                table: "contracts");

            migrationBuilder.DropColumn(
                name: "trips_per_month",
                table: "contracts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "max_kms",
                table: "contracts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "trips_per_day",
                table: "contracts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "trips_per_month",
                table: "contracts",
                type: "integer",
                nullable: true);
        }
    }
}
