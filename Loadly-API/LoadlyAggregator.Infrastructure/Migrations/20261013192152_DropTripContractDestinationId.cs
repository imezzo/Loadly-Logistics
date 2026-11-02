using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropTripContractDestinationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_contract_destinations_contract_destination_id",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_contract_destination_id",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "contract_destination_id",
                table: "trips");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<long>(
                name: "contract_destination_id",
                table: "trips",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trips_contract_destination_id",
                table: "trips",
                column: "contract_destination_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_contract_destinations_contract_destination_id",
                table: "trips",
                column: "contract_destination_id",
                principalTable: "contract_destinations",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
