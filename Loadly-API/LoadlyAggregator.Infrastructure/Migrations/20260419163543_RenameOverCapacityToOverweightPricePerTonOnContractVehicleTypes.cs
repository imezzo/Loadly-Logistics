using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameOverCapacityToOverweightPricePerTonOnContractVehicleTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "over_capacity",
                table: "contract_vehicle_types",
                newName: "overweight_price_per_ton");

            migrationBuilder.AlterColumn<decimal>(
                name: "overweight_price_per_ton",
                table: "contract_vehicle_types",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "overweight_price_per_ton",
                table: "contract_vehicle_types",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.RenameColumn(
                name: "overweight_price_per_ton",
                table: "contract_vehicle_types",
                newName: "over_capacity");
        }
    }
}
