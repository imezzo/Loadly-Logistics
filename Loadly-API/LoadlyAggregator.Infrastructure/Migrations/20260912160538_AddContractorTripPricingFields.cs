using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractorTripPricingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "contractor_price",
                table: "trips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "contractor_overweight_price_per_ton",
                table: "contract_vehicle_types",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "contractor_overweight_price_per_ton",
                table: "contract_slots",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "contractor_fixed_trip_price",
                table: "contract_destinations",
                type: "numeric(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "contractor_price_per_ton",
                table: "contract_destinations",
                type: "numeric(10,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "contractor_price",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "contractor_overweight_price_per_ton",
                table: "contract_vehicle_types");

            migrationBuilder.DropColumn(
                name: "contractor_overweight_price_per_ton",
                table: "contract_slots");

            migrationBuilder.DropColumn(
                name: "contractor_fixed_trip_price",
                table: "contract_destinations");

            migrationBuilder.DropColumn(
                name: "contractor_price_per_ton",
                table: "contract_destinations");
        }
    }
}
