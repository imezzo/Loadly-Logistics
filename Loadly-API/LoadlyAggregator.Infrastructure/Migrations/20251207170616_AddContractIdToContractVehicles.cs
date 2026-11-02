using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddContractIdToContractVehicles : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "contract_id",
            table: "contract_vehicles",
            type: "bigint",
            nullable: false,
            defaultValue: 0L);

        migrationBuilder.CreateIndex(
            name: "IX_contract_vehicles_contract_id",
            table: "contract_vehicles",
            column: "contract_id");

        migrationBuilder.AddForeignKey(
            name: "FK_contract_vehicles_contracts_contract_id",
            table: "contract_vehicles",
            column: "contract_id",
            principalTable: "contracts",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_contract_vehicles_contracts_contract_id",
            table: "contract_vehicles");

        migrationBuilder.DropIndex(
            name: "IX_contract_vehicles_contract_id",
            table: "contract_vehicles");

        migrationBuilder.DropColumn(
            name: "contract_id",
            table: "contract_vehicles");
    }
}
