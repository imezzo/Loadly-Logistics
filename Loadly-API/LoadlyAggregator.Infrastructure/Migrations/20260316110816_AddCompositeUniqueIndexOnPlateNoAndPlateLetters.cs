using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositeUniqueIndexOnPlateNoAndPlateLetters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_vehicles_plate_no",
                table: "vehicles");

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_plate_no_plate_letters",
                table: "vehicles",
                columns: new[] { "plate_no", "plate_letters" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_vehicles_plate_no_plate_letters",
                table: "vehicles");

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_plate_no",
                table: "vehicles",
                column: "plate_no");
        }
    }
}
