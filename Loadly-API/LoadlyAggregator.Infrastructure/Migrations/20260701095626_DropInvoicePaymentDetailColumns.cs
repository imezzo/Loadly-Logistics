using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropInvoicePaymentDetailColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_invoice_payments_payment_date",
                table: "invoice_payments");

            migrationBuilder.DropColumn(
                name: "payment_attachment",
                table: "invoice_payments");

            migrationBuilder.DropColumn(
                name: "payment_date",
                table: "invoice_payments");

            migrationBuilder.DropColumn(
                name: "payment_method",
                table: "invoice_payments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "payment_attachment",
                table: "invoice_payments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "payment_date",
                table: "invoice_payments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "payment_method",
                table: "invoice_payments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_invoice_payments_payment_date",
                table: "invoice_payments",
                column: "payment_date");
        }
    }
}
