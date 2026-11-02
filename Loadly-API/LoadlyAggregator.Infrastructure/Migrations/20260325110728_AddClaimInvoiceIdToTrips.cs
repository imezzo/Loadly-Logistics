using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimInvoiceIdToTrips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "claim_invoice_id",
                table: "trips",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trips_claim_invoice_id",
                table: "trips",
                column: "claim_invoice_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_invoices_claim_invoice_id",
                table: "trips",
                column: "claim_invoice_id",
                principalTable: "invoices",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_invoices_claim_invoice_id",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_claim_invoice_id",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "claim_invoice_id",
                table: "trips");
        }
    }
}
