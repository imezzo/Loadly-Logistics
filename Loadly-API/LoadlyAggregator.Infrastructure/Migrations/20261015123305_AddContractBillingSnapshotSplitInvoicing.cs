using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractBillingSnapshotSplitInvoicing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "claim_invoice_id",
                table: "contract_billing_snapshots",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "extra_km_overage_invoice_type",
                table: "contract_billing_snapshots",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "claim_invoice_id",
                table: "contract_billing_snapshots");

            migrationBuilder.DropColumn(
                name: "extra_km_overage_invoice_type",
                table: "contract_billing_snapshots");
        }
    }
}
