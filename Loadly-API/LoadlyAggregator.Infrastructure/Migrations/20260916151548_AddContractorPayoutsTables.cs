using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractorPayoutsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contractor_payouts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    contractor_id = table.Column<long>(type: "bigint", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    added_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contractor_payouts", x => x.id);
                    table.ForeignKey(
                        name: "FK_contractor_payouts_contractors_contractor_id",
                        column: x => x.contractor_id,
                        principalTable: "contractors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddColumn<long>(
                name: "contractor_payout_id",
                table: "trips",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_contractor_payouts_contractor_id",
                table: "contractor_payouts",
                column: "contractor_id");

            migrationBuilder.CreateIndex(
                name: "IX_contractor_payouts_contractor_id_status",
                table: "contractor_payouts",
                columns: new[] { "contractor_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_contractor_payouts_status",
                table: "contractor_payouts",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_trips_contractor_payout_id",
                table: "trips",
                column: "contractor_payout_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_contractor_payouts_contractor_payout_id",
                table: "trips",
                column: "contractor_payout_id",
                principalTable: "contractor_payouts",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_contractor_payouts_contractor_payout_id",
                table: "trips");

            migrationBuilder.DropTable(
                name: "contractor_payouts");

            migrationBuilder.DropIndex(
                name: "IX_trips_contractor_payout_id",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "contractor_payout_id",
                table: "trips");
        }
    }
}
