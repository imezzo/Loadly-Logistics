using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddSalesRepIdToContracts : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "sales_rep_id",
            table: "contracts",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_contracts_sales_rep_id",
            table: "contracts",
            column: "sales_rep_id");

        migrationBuilder.AddForeignKey(
            name: "FK_contracts_admins_sales_rep_id",
            table: "contracts",
            column: "sales_rep_id",
            principalTable: "admins",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_contracts_admins_sales_rep_id",
            table: "contracts");

        migrationBuilder.DropIndex(
            name: "IX_contracts_sales_rep_id",
            table: "contracts");

        migrationBuilder.DropColumn(
            name: "sales_rep_id",
            table: "contracts");
    }
}
