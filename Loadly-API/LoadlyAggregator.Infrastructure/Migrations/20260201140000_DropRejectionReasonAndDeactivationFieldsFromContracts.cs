using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class DropRejectionReasonAndDeactivationFieldsFromContracts : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "rejection_reason",
            table: "contracts");

        migrationBuilder.DropColumn(
            name: "deactivation_reason",
            table: "contracts");

        migrationBuilder.DropColumn(
            name: "deactivation_notes",
            table: "contracts");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "rejection_reason",
            table: "contracts",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "deactivation_reason",
            table: "contracts",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "deactivation_notes",
            table: "contracts",
            type: "text",
            nullable: true);
    }
}
