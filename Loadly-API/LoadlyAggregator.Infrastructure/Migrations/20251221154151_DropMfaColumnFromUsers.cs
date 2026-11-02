using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class DropMfaColumnFromUsers : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "mfa",
            table: "drivers");

        migrationBuilder.DropColumn(
            name: "mfa",
            table: "companies");

        migrationBuilder.DropColumn(
            name: "mfa",
            table: "admins");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "mfa",
            table: "drivers",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "mfa",
            table: "companies",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "mfa",
            table: "admins",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }
}
