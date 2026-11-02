using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddLicenseAndStatusAndLanguageFieldsToDriverDetails : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "emergency_contact_name",
            table: "driver_details",
            type: "character varying(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "emergency_contact_phone",
            table: "driver_details",
            type: "character varying(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "license_category",
            table: "driver_details",
            type: "integer",
            nullable: true,
            defaultValue: 1);

        migrationBuilder.AddColumn<int[]>(
            name: "spoken_languages",
            table: "driver_details",
            type: "integer[]",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "working_status",
            table: "driver_details",
            type: "integer",
            nullable: true,
            defaultValue: 1);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "emergency_contact_name",
            table: "driver_details");

        migrationBuilder.DropColumn(
            name: "emergency_contact_phone",
            table: "driver_details");

        migrationBuilder.DropColumn(
            name: "license_category",
            table: "driver_details");

        migrationBuilder.DropColumn(
            name: "spoken_languages",
            table: "driver_details");

        migrationBuilder.DropColumn(
            name: "working_status",
            table: "driver_details");
    }
}
