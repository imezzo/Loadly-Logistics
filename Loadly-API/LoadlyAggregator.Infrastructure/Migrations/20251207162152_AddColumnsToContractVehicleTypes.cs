using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddColumnsToContractVehicleTypes : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "added_by",
            table: "contract_vehicle_types",
            type: "bigint",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "auto_assign_mode",
            table: "contract_vehicle_types",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<decimal>(
            name: "min_capacity",
            table: "contract_vehicle_types",
            type: "numeric(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "over_capacity",
            table: "contract_vehicle_types",
            type: "numeric(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "quantity",
            table: "contract_vehicle_types",
            type: "integer",
            nullable: false,
            defaultValue: 1);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "added_by",
            table: "contract_vehicle_types");

        migrationBuilder.DropColumn(
            name: "auto_assign_mode",
            table: "contract_vehicle_types");

        migrationBuilder.DropColumn(
            name: "min_capacity",
            table: "contract_vehicle_types");

        migrationBuilder.DropColumn(
            name: "over_capacity",
            table: "contract_vehicle_types");

        migrationBuilder.DropColumn(
            name: "quantity",
            table: "contract_vehicle_types");
    }
}
