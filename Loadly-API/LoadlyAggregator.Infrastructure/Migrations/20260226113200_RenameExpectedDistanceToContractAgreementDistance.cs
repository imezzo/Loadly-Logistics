using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RenameExpectedDistanceToContractAgreementDistance : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "expected_distance_km",
            table: "trips",
            newName: "contract_distance_km");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "contract_distance_km",
            table: "trips",
            newName: "expected_distance_km");
    }
}
