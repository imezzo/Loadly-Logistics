using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddContractTypeIdToContracts : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "contract_type_id",
            table: "contracts",
            type: "bigint",
            nullable: false,
            defaultValue: 0L);

        migrationBuilder.CreateIndex(
            name: "IX_contracts_contract_type_id",
            table: "contracts",
            column: "contract_type_id");

        migrationBuilder.AddForeignKey(
            name: "FK_contracts_contract_types_contract_type_id",
            table: "contracts",
            column: "contract_type_id",
            principalTable: "contract_types",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_contracts_contract_types_contract_type_id",
            table: "contracts");

        migrationBuilder.DropIndex(
            name: "IX_contracts_contract_type_id",
            table: "contracts");

        migrationBuilder.DropColumn(
            name: "contract_type_id",
            table: "contracts");
    }
}
