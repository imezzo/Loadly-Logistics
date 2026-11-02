using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCollectorIdToTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "collector_id",
                table: "trips",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "documents_approved",
                table: "trips",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "collector_id",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "documents_approved",
                table: "trips");
        }
    }
}
