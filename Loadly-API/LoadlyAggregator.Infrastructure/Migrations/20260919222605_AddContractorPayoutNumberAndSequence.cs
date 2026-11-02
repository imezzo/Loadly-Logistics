using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractorPayoutNumberAndSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "payout_number",
                table: "contractor_payouts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "sequence",
                table: "contractor_payouts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_contractor_payouts_payout_number",
                table: "contractor_payouts",
                column: "payout_number",
                unique: true,
                filter: "\"deleted_at\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_contractor_payouts_payout_number",
                table: "contractor_payouts");

            migrationBuilder.DropColumn(
                name: "payout_number",
                table: "contractor_payouts");

            migrationBuilder.DropColumn(
                name: "sequence",
                table: "contractor_payouts");
        }
    }
}
