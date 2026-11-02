using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddPaymentDueDaysToContracts : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<short>(
            name: "payment_due_days",
            table: "contracts",
            type: "smallint",
            nullable: false,
            defaultValue: (short)0);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "payment_due_days",
            table: "contracts");
    }
}
