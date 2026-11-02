using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayerToBankTransfers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "payer_id",
                table: "bank_transfers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payer_type",
                table: "bank_transfers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_bank_transfers_payer_id_payer_type",
                table: "bank_transfers",
                columns: new[] { "payer_id", "payer_type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_bank_transfers_payer_id_payer_type",
                table: "bank_transfers");

            migrationBuilder.DropColumn(
                name: "payer_id",
                table: "bank_transfers");

            migrationBuilder.DropColumn(
                name: "payer_type",
                table: "bank_transfers");
        }
    }
}
