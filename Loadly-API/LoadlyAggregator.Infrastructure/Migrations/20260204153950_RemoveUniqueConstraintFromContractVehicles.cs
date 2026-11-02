using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveUniqueConstraintFromContractVehicles : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_contract_vehicles_unique_combination",
            table: "contract_vehicles");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "ix_contract_vehicles_unique_combination",
            table: "contract_vehicles",
            columns: new[] { "contract_id", "vehicle_id", "vehicle_tail_id" },
            unique: true);
    }
}
