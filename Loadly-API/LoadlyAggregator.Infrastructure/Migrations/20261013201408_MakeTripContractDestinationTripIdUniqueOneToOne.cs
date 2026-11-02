using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeTripContractDestinationTripIdUniqueOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_trip_contract_destinations_trip_id",
                table: "trip_contract_destinations");

            migrationBuilder.CreateIndex(
                name: "IX_trip_contract_destinations_trip_id",
                table: "trip_contract_destinations",
                column: "trip_id",
                unique: true,
                filter: "\"deleted_at\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_trip_contract_destinations_trip_id",
                table: "trip_contract_destinations");

            migrationBuilder.CreateIndex(
                name: "IX_trip_contract_destinations_trip_id",
                table: "trip_contract_destinations",
                column: "trip_id");
        }
    }
}
