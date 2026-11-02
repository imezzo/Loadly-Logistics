using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddActiveVehicleUniqueIndexToContractSlots : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "ix_contract_slots_active_vehicle_id",
            table: "contract_slots",
            column: "vehicle_id",
            unique: true,
            filter: "\"deleted_at\" IS NULL AND \"vehicle_id\" IS NOT NULL AND \"status\" = 1");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_contract_slots_active_vehicle_id",
            table: "contract_slots");
    }
}
