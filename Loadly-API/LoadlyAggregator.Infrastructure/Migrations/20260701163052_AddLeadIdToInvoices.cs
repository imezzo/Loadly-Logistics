using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadIdToInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "lead_id",
                table: "invoices",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_invoices_lead_id",
                table: "invoices",
                column: "lead_id");

            migrationBuilder.AddForeignKey(
                name: "FK_invoices_leads_lead_id",
                table: "invoices",
                column: "lead_id",
                principalTable: "leads",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoices_leads_lead_id",
                table: "invoices");

            migrationBuilder.DropIndex(
                name: "IX_invoices_lead_id",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "lead_id",
                table: "invoices");
        }
    }
}
