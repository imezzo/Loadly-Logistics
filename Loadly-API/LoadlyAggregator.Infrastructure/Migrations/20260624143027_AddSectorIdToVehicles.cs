using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSectorIdToVehicles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_contractors_contractor_id",
                table: "vehicles");

            migrationBuilder.AddColumn<long>(
                name: "sector_id",
                table: "vehicles",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_sector_id",
                table: "vehicles",
                column: "sector_id");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_contractors_contractor_id",
                table: "vehicles",
                column: "contractor_id",
                principalTable: "contractors",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_sectors_sector_id",
                table: "vehicles",
                column: "sector_id",
                principalTable: "sectors",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_contractors_contractor_id",
                table: "vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_sectors_sector_id",
                table: "vehicles");

            migrationBuilder.DropIndex(
                name: "IX_vehicles_sector_id",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "sector_id",
                table: "vehicles");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_contractors_contractor_id",
                table: "vehicles",
                column: "contractor_id",
                principalTable: "contractors",
                principalColumn: "id");
        }
    }
}
