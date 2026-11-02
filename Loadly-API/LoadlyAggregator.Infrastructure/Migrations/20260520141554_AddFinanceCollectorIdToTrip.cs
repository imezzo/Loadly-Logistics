using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinanceCollectorIdToTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "finance_collector_id",
                table: "trips",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trips_finance_collector_id",
                table: "trips",
                column: "finance_collector_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_admins_finance_collector_id",
                table: "trips",
                column: "finance_collector_id",
                principalTable: "admins",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_admins_finance_collector_id",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_finance_collector_id",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "finance_collector_id",
                table: "trips");
        }
    }
}
