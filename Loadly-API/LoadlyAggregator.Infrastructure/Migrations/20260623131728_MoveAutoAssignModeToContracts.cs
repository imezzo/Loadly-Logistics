using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveAutoAssignModeToContracts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "auto_assign_mode",
                table: "contract_vehicle_types");

            migrationBuilder.AddColumn<bool>(
                name: "auto_assign_mode",
                table: "contracts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "auto_assign_mode",
                table: "contracts");

            migrationBuilder.AddColumn<bool>(
                name: "auto_assign_mode",
                table: "contract_vehicle_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
