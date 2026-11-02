using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class RenameDriverFullNameToFullNameEnAndAddFullNameAr : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "full_name",
            table: "drivers",
            newName: "fullname_en");

        migrationBuilder.AddColumn<string>(
            name: "fullname_ar",
            table: "drivers",
            type: "character varying(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "");


        migrationBuilder.Sql("UPDATE drivers SET fullname_ar = fullname_en WHERE fullname_ar = '';");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "fullname_ar",
            table: "drivers");

        migrationBuilder.RenameColumn(
            name: "fullname_en",
            table: "drivers",
            newName: "full_name");
    }
}
