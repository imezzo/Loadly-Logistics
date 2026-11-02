using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddReferenceNumberToContracts : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "reference_number",
            table: "contracts",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "reference_sequence",
            table: "contracts",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "IX_contracts_reference_number",
            table: "contracts",
            column: "reference_number",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_contracts_reference_sequence",
            table: "contracts",
            column: "reference_sequence");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_contracts_reference_number",
            table: "contracts");

        migrationBuilder.DropIndex(
            name: "IX_contracts_reference_sequence",
            table: "contracts");

        migrationBuilder.DropColumn(
            name: "reference_number",
            table: "contracts");

        migrationBuilder.DropColumn(
            name: "reference_sequence",
            table: "contracts");
    }
}
