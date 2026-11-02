using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoicePaymentInvoiceIdBankTransferIdUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_invoice_payments_invoice_id_bank_transfer_id",
                table: "invoice_payments",
                columns: new[] { "invoice_id", "bank_transfer_id" },
                unique: true,
                filter: "\"deleted_at\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_invoice_payments_invoice_id_bank_transfer_id",
                table: "invoice_payments");
        }
    }
}
