using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyIdToVehicleFacilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "company_id",
                table: "vehicle_facilities",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_facilities_company_id",
                table: "vehicle_facilities",
                column: "company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicle_facilities_companies_company_id",
                table: "vehicle_facilities",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicle_facilities_companies_company_id",
                table: "vehicle_facilities");

            migrationBuilder.DropIndex(
                name: "IX_vehicle_facilities_company_id",
                table: "vehicle_facilities");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "vehicle_facilities");
        }
    }
}
