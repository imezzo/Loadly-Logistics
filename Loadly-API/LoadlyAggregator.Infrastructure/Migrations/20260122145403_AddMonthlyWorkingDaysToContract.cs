using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddMonthlyWorkingDaysToContract : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "monthly_working_days",
            table: "contracts",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "monthly_working_days",
            table: "contracts");
    }
}
