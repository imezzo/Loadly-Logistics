using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveUniqueConstraintFromContractNames : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_contracts_name_ar",
            table: "contracts");

        migrationBuilder.DropIndex(
            name: "IX_contracts_name_en",
            table: "contracts");

        migrationBuilder.CreateIndex(
            name: "IX_contracts_name_ar",
            table: "contracts",
            column: "name_ar");

        migrationBuilder.CreateIndex(
            name: "IX_contracts_name_en",
            table: "contracts",
            column: "name_en");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_contracts_name_ar",
            table: "contracts");

        migrationBuilder.DropIndex(
            name: "IX_contracts_name_en",
            table: "contracts");

        migrationBuilder.CreateIndex(
            name: "IX_contracts_name_ar",
            table: "contracts",
            column: "name_ar",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_contracts_name_en",
            table: "contracts",
            column: "name_en",
            unique: true);
    }
}
