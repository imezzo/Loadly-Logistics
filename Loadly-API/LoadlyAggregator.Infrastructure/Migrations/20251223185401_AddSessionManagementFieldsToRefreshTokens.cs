using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddSessionManagementFieldsToRefreshTokens : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ip_address",
            table: "refresh_tokens",
            type: "character varying(45)",
            maxLength: 45,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "last_used_at",
            table: "refresh_tokens",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "session_name",
            table: "refresh_tokens",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "user_agent",
            table: "refresh_tokens",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ip_address",
            table: "refresh_tokens");

        migrationBuilder.DropColumn(
            name: "last_used_at",
            table: "refresh_tokens");

        migrationBuilder.DropColumn(
            name: "session_name",
            table: "refresh_tokens");

        migrationBuilder.DropColumn(
            name: "user_agent",
            table: "refresh_tokens");
    }
}
