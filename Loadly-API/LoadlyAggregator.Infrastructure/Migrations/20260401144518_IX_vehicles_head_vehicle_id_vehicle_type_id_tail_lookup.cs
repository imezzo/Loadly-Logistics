using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IX_vehicles_head_vehicle_id_vehicle_type_id_tail_lookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_vehicles_head_vehicle_id_vehicle_type_id_tail_lookup",
                table: "vehicles",
                columns: new[] { "head_vehicle_id", "vehicle_type_id" },
                filter: "head_vehicle_id IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_vehicles_head_vehicle_id_vehicle_type_id_tail_lookup",
                table: "vehicles");
        }
    }
}
