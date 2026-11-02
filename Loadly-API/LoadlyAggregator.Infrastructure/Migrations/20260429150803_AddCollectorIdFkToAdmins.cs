using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCollectorIdFkToAdmins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_trips_collector_id",
                table: "trips",
                column: "collector_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_admins_collector_id",
                table: "trips",
                column: "collector_id",
                principalTable: "admins",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_admins_collector_id",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_collector_id",
                table: "trips");
        }
    }
}
