using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class DropContractIdForeignKeyFromContractPackages : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_contract_packages_company_contracts_contract_id",
            table: "contract_packages");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddForeignKey(
            name: "FK_contract_packages_company_contracts_contract_id",
            table: "contract_packages",
            column: "contract_id",
            principalTable: "company_contracts",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }
}
