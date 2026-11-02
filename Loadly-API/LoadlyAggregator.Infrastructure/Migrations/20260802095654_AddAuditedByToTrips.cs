using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditedByToTrips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "audited_at",
                table: "trips",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "audited_by_id",
                table: "trips",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trips_audited_by_id",
                table: "trips",
                column: "audited_by_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_admins_audited_by_id",
                table: "trips",
                column: "audited_by_id",
                principalTable: "admins",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_admins_audited_by_id",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_audited_by_id",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "audited_at",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "audited_by_id",
                table: "trips");
        }
    }
}
