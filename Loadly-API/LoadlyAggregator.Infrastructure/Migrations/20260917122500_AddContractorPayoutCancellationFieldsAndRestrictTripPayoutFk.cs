using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractorPayoutCancellationFieldsAndRestrictTripPayoutFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_contractor_payouts_contractor_payout_id",
                table: "trips");

            migrationBuilder.AddColumn<string>(
                name: "cancellation_reason",
                table: "contractor_payouts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "cancelled_at",
                table: "contractor_payouts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "cancelled_by",
                table: "contractor_payouts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_trips_contractor_payouts_contractor_payout_id",
                table: "trips",
                column: "contractor_payout_id",
                principalTable: "contractor_payouts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_contractor_payouts_contractor_payout_id",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "cancellation_reason",
                table: "contractor_payouts");

            migrationBuilder.DropColumn(
                name: "cancelled_at",
                table: "contractor_payouts");

            migrationBuilder.DropColumn(
                name: "cancelled_by",
                table: "contractor_payouts");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_contractor_payouts_contractor_payout_id",
                table: "trips",
                column: "contractor_payout_id",
                principalTable: "contractor_payouts",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
