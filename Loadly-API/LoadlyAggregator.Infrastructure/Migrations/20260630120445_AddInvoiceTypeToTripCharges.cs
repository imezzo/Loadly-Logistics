using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceTypeToTripCharges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "invoice_type",
                table: "trip_charges",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "invoice_type",
                table: "trip_charges");
        }
    }
}
