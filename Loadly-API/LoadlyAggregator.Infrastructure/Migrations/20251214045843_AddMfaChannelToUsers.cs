using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddMfaChannelToUsers : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "mfa_channel",
            table: "drivers",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "mfa_channel",
            table: "companies",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "mfa_channel",
            table: "admins",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "mfa_channel",
            table: "drivers");

        migrationBuilder.DropColumn(
            name: "mfa_channel",
            table: "companies");

        migrationBuilder.DropColumn(
            name: "mfa_channel",
            table: "admins");
    }
}
