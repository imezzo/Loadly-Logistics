using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddNationalIdToDriversTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "national_id",
            table: "drivers",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "national_id",
            table: "drivers");
    }
}
