using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCityNavigationPropertyToDistrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_vehicle_details_vehicle_id",
                table: "vehicle_details");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_details_vehicle_id",
                table: "vehicle_details",
                column: "vehicle_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_districts_cities_city_id",
                table: "districts",
                column: "city_id",
                principalTable: "cities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_districts_cities_city_id",
                table: "districts");

            migrationBuilder.DropIndex(
                name: "IX_vehicle_details_vehicle_id",
                table: "vehicle_details");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_details_vehicle_id",
                table: "vehicle_details",
                column: "vehicle_id");
        }
    }
}
