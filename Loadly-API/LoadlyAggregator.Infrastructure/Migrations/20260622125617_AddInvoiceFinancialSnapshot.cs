using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceFinancialSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "discount_amount",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "discount_type",
                table: "invoices",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "discount_value",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "tax_amount",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "tax_type",
                table: "invoices",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "tax_value",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: true);

            // Best-effort backfill for invoices issued before snapshotting existed.
            // The pre-snapshot model only stored FKs, so the current master rate/type is the
            // only source available; computed amounts are derived from the stored sub_total.
            // tax/discount type: 0 = Fixed, 1 = Percentage.
            migrationBuilder.Sql(@"
                UPDATE invoices i
                SET tax_type = t.type,
                    tax_value = t.value,
                    tax_amount = CASE WHEN t.type = 0 THEN t.value ELSE i.sub_total * t.value / 100 END
                FROM taxes t
                WHERE i.tax_id = t.id;");

            migrationBuilder.Sql(@"
                UPDATE invoices i
                SET discount_type = d.type,
                    discount_value = d.value,
                    discount_amount = CASE WHEN d.type = 0 THEN d.value ELSE i.sub_total * d.value / 100 END
                FROM discounts d
                WHERE i.discount_id = d.id;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "discount_amount",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "discount_type",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "discount_value",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "tax_amount",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "tax_type",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "tax_value",
                table: "invoices");
        }
    }
}
