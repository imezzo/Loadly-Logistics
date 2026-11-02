using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialDateToTrips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "financial_date",
                table: "trips",
                type: "date",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trips_financial_date",
                table: "trips",
                column: "financial_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_trips_financial_date",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "financial_date",
                table: "trips");
        }
    }
}
