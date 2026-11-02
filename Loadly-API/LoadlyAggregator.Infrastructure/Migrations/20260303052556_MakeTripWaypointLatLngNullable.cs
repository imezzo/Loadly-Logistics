using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeTripWaypointLatLngNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "longitude",
                table: "trip_waypoints",
                type: "numeric(11,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(11,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "latitude",
                table: "trip_waypoints",
                type: "numeric(10,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,8)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "longitude",
                table: "trip_waypoints",
                type: "numeric(11,8)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(11,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "latitude",
                table: "trip_waypoints",
                type: "numeric(10,8)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,8)",
                oldNullable: true);
        }
    }
}
