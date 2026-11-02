using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddInvoiceTables : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "invoice_id",
            table: "trips",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "invoices",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                invoice_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                sequence = table.Column<int>(type: "integer", nullable: true),
                invoice_type = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                company_id = table.Column<long>(type: "bigint", nullable: false),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                period = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                sub_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                tax_id = table.Column<long>(type: "bigint", nullable: true),
                discount_id = table.Column<long>(type: "bigint", nullable: true),
                total_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                taxed_invoice_attachment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_invoices", x => x.id);
                table.ForeignKey(
                    name: "FK_invoices_companies_company_id",
                    column: x => x.company_id,
                    principalTable: "companies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_invoices_discounts_discount_id",
                    column: x => x.discount_id,
                    principalTable: "discounts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_invoices_taxes_tax_id",
                    column: x => x.tax_id,
                    principalTable: "taxes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "invoice_payment_requests",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                invoice_id = table.Column<long>(type: "bigint", nullable: false),
                amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                request_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                attachment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                payment_method = table.Column<int>(type: "integer", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                rejection_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_invoice_payment_requests", x => x.id);
                table.ForeignKey(
                    name: "FK_invoice_payment_requests_invoices_invoice_id",
                    column: x => x.invoice_id,
                    principalTable: "invoices",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "invoice_payments",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                invoice_id = table.Column<long>(type: "bigint", nullable: false),
                amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                payment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                payment_method = table.Column<int>(type: "integer", nullable: false),
                payment_attachment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_invoice_payments", x => x.id);
                table.ForeignKey(
                    name: "FK_invoice_payments_invoices_invoice_id",
                    column: x => x.invoice_id,
                    principalTable: "invoices",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_trips_invoice_id",
            table: "trips",
            column: "invoice_id");

        migrationBuilder.CreateIndex(
            name: "IX_invoice_payment_requests_invoice_id",
            table: "invoice_payment_requests",
            column: "invoice_id");

        migrationBuilder.CreateIndex(
            name: "IX_invoice_payment_requests_invoice_id_status",
            table: "invoice_payment_requests",
            columns: new[] { "invoice_id", "status" });

        migrationBuilder.CreateIndex(
            name: "IX_invoice_payment_requests_request_date",
            table: "invoice_payment_requests",
            column: "request_date");

        migrationBuilder.CreateIndex(
            name: "IX_invoice_payment_requests_status",
            table: "invoice_payment_requests",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "IX_invoice_payments_invoice_id",
            table: "invoice_payments",
            column: "invoice_id");

        migrationBuilder.CreateIndex(
            name: "IX_invoice_payments_payment_date",
            table: "invoice_payments",
            column: "payment_date");

        migrationBuilder.CreateIndex(
            name: "IX_invoices_company_id",
            table: "invoices",
            column: "company_id");

        migrationBuilder.CreateIndex(
            name: "IX_invoices_company_id_status",
            table: "invoices",
            columns: new[] { "company_id", "status" });

        migrationBuilder.CreateIndex(
            name: "IX_invoices_discount_id",
            table: "invoices",
            column: "discount_id");

        migrationBuilder.CreateIndex(
            name: "IX_invoices_end_date",
            table: "invoices",
            column: "end_date");

        migrationBuilder.CreateIndex(
            name: "IX_invoices_invoice_type",
            table: "invoices",
            column: "invoice_type");

        migrationBuilder.CreateIndex(
            name: "IX_invoices_start_date",
            table: "invoices",
            column: "start_date");

        migrationBuilder.CreateIndex(
            name: "IX_invoices_status",
            table: "invoices",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "IX_invoices_tax_id",
            table: "invoices",
            column: "tax_id");

        migrationBuilder.AddForeignKey(
            name: "FK_trips_invoices_invoice_id",
            table: "trips",
            column: "invoice_id",
            principalTable: "invoices",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_trips_invoices_invoice_id",
            table: "trips");

        migrationBuilder.DropTable(
            name: "invoice_payment_requests");

        migrationBuilder.DropTable(
            name: "invoice_payments");

        migrationBuilder.DropTable(
            name: "invoices");

        migrationBuilder.DropIndex(
            name: "IX_trips_invoice_id",
            table: "trips");

        migrationBuilder.DropColumn(
            name: "invoice_id",
            table: "trips");
    }
}
