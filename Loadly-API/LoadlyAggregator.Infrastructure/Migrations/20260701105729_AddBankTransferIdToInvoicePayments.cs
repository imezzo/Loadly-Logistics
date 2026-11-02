using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBankTransferIdToInvoicePayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "bank_transfer_id",
                table: "invoice_payments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_invoice_payments_bank_transfer_id",
                table: "invoice_payments",
                column: "bank_transfer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_invoice_payments_bank_transfers_bank_transfer_id",
                table: "invoice_payments",
                column: "bank_transfer_id",
                principalTable: "bank_transfers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoice_payments_bank_transfers_bank_transfer_id",
                table: "invoice_payments");

            migrationBuilder.DropIndex(
                name: "IX_invoice_payments_bank_transfer_id",
                table: "invoice_payments");

            migrationBuilder.DropColumn(
                name: "bank_transfer_id",
                table: "invoice_payments");
        }
    }
}
