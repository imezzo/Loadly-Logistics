using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadIdToTrips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "lead_id",
                table: "trips",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trips_lead_id",
                table: "trips",
                column: "lead_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_leads_lead_id",
                table: "trips",
                column: "lead_id",
                principalTable: "leads",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_leads_lead_id",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_lead_id",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "lead_id",
                table: "trips");
        }
    }
}
