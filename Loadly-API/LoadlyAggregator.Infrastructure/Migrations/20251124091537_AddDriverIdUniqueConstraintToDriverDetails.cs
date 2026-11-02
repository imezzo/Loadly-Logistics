using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddDriverIdUniqueConstraintToDriverDetails : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_driver_details_driver_id",
            table: "driver_details");

        migrationBuilder.CreateIndex(
            name: "IX_driver_details_driver_id",
            table: "driver_details",
            column: "driver_id",
            unique: true);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_driver_details_driver_id",
            table: "driver_details");

        migrationBuilder.CreateIndex(
            name: "IX_driver_details_driver_id",
            table: "driver_details",
            column: "driver_id");
    }
}
