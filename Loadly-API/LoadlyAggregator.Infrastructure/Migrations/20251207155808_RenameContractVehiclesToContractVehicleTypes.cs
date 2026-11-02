using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class RenameContractVehiclesToContractVehicleTypes : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_contract_vehicles_contracts_contract_id",
            table: "contract_vehicles");

        migrationBuilder.DropForeignKey(
            name: "FK_contract_vehicles_vehicle_types_vehicle_type_id",
            table: "contract_vehicles");

        migrationBuilder.DropPrimaryKey(
            name: "PK_contract_vehicles",
            table: "contract_vehicles");

        migrationBuilder.RenameTable(
            name: "contract_vehicles",
            newName: "contract_vehicle_types");

        migrationBuilder.RenameIndex(
            name: "IX_contract_vehicles_vehicle_type_id",
            table: "contract_vehicle_types",
            newName: "IX_contract_vehicle_types_vehicle_type_id");

        migrationBuilder.RenameIndex(
            name: "IX_contract_vehicles_contract_id",
            table: "contract_vehicle_types",
            newName: "IX_contract_vehicle_types_contract_id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_contract_vehicle_types",
            table: "contract_vehicle_types",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "FK_contract_vehicle_types_contracts_contract_id",
            table: "contract_vehicle_types",
            column: "contract_id",
            principalTable: "contracts",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_contract_vehicle_types_vehicle_types_vehicle_type_id",
            table: "contract_vehicle_types",
            column: "vehicle_type_id",
            principalTable: "vehicle_types",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_contract_vehicle_types_contracts_contract_id",
            table: "contract_vehicle_types");

        migrationBuilder.DropForeignKey(
            name: "FK_contract_vehicle_types_vehicle_types_vehicle_type_id",
            table: "contract_vehicle_types");

        migrationBuilder.DropPrimaryKey(
            name: "PK_contract_vehicle_types",
            table: "contract_vehicle_types");

        migrationBuilder.RenameTable(
            name: "contract_vehicle_types",
            newName: "contract_vehicles");

        migrationBuilder.RenameIndex(
            name: "IX_contract_vehicle_types_vehicle_type_id",
            table: "contract_vehicles",
            newName: "IX_contract_vehicles_vehicle_type_id");

        migrationBuilder.RenameIndex(
            name: "IX_contract_vehicle_types_contract_id",
            table: "contract_vehicles",
            newName: "IX_contract_vehicles_contract_id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_contract_vehicles",
            table: "contract_vehicles",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "FK_contract_vehicles_contracts_contract_id",
            table: "contract_vehicles",
            column: "contract_id",
            principalTable: "contracts",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_contract_vehicles_vehicle_types_vehicle_type_id",
            table: "contract_vehicles",
            column: "vehicle_type_id",
            principalTable: "vehicle_types",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }
}
