using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddCompanyIdToCompaniesTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "company_id",
            table: "companies",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_companies_company_id",
            table: "companies",
            column: "company_id");

        migrationBuilder.AddForeignKey(
            name: "FK_companies_companies_company_id",
            table: "companies",
            column: "company_id",
            principalTable: "companies",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_companies_companies_company_id",
            table: "companies");

        migrationBuilder.DropIndex(
            name: "IX_companies_company_id",
            table: "companies");

        migrationBuilder.DropColumn(
            name: "company_id",
            table: "companies");
    }
}
