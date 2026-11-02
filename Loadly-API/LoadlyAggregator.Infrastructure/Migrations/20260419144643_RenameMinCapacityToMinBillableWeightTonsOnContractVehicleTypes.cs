using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameMinCapacityToMinBillableWeightTonsOnContractVehicleTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "min_capacity",
                table: "contract_vehicle_types",
                newName: "min_billable_weight_tons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "min_billable_weight_tons",
                table: "contract_vehicle_types",
                newName: "min_capacity");
        }
    }
}
