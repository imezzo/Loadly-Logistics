using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class UpdateContractVehicleToVehicleIdAndTail : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "vehicle_tail_id",
            table: "contract_vehicles",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_contract_vehicles_unique_combination",
            table: "contract_vehicles",
            columns: new[] { "contract_id", "vehicle_id", "vehicle_tail_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_contract_vehicles_vehicle_tail_id",
            table: "contract_vehicles",
            column: "vehicle_tail_id");

        migrationBuilder.AddForeignKey(
            name: "FK_contract_vehicles_vehicles_vehicle_tail_id",
            table: "contract_vehicles",
            column: "vehicle_tail_id",
            principalTable: "vehicles",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_contract_vehicles_vehicles_vehicle_tail_id",
            table: "contract_vehicles");

        migrationBuilder.DropIndex(
            name: "ix_contract_vehicles_unique_combination",
            table: "contract_vehicles");

        migrationBuilder.DropIndex(
            name: "IX_contract_vehicles_vehicle_tail_id",
            table: "contract_vehicles");

        migrationBuilder.DropColumn(
            name: "vehicle_tail_id",
            table: "contract_vehicles");
    }
}
