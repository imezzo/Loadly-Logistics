using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddLicenseTypeIdForeignKeyToDriverDetails : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddForeignKey(
            name: "FK_driver_details_driver_license_types_license_type_id",
            table: "driver_details",
            column: "license_type_id",
            principalTable: "driver_license_types",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_driver_details_driver_license_types_license_type_id",
            table: "driver_details");
    }
}
