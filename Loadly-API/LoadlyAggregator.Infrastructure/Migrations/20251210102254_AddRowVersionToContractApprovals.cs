using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddRowVersionToContractApprovals : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<byte[]>(
            name: "row_version",
            table: "contract_approvals",
            type: "bytea",
            rowVersion: true,
            nullable: false,
            defaultValue: new byte[0]);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "row_version",
            table: "contract_approvals");
    }
}
