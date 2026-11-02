using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class MakeConsumptionRateNullableInContractVehicleTypes : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "consumption_rate",
            table: "contract_vehicle_types",
            type: "numeric(18,2)",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "numeric(18,2)");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "consumption_rate",
            table: "contract_vehicle_types",
            type: "numeric(18,2)",
            nullable: false,
            defaultValue: 0m,
            oldClrType: typeof(decimal),
            oldType: "numeric(18,2)",
            oldNullable: true);
    }
}
