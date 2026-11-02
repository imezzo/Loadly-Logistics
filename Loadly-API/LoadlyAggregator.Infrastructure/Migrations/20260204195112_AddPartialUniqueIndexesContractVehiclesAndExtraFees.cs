using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddPartialUniqueIndexesContractVehiclesAndExtraFees : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "ix_contract_vehicles_unique_combination",
            table: "contract_vehicles",
            columns: new[] { "contract_id", "vehicle_id", "vehicle_tail_id" },
            unique: true,
            filter: "\"deleted_at\" IS NULL");

        migrationBuilder.CreateIndex(
            name: "ix_contract_extra_fees_unique_combination",
            table: "contract_extra_fees",
            columns: new[] { "contract_id", "extra_fee_type_id" },
            unique: true,
            filter: "\"deleted_at\" IS NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_contract_vehicles_unique_combination",
            table: "contract_vehicles");

        migrationBuilder.DropIndex(
            name: "ix_contract_extra_fees_unique_combination",
            table: "contract_extra_fees");
    }
}
