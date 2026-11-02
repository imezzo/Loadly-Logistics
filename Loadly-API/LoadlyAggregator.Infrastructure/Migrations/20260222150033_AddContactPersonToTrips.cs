using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddContactPersonToTrips : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "contact_person_mobile",
            table: "trips",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "contact_person_name",
            table: "trips",
            type: "character varying(255)",
            maxLength: 255,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "contact_person_mobile",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "contact_person_name",
            table: "trips");
    }
}
