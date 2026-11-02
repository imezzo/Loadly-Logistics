using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceCalendarDayTripCountWithTripIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "trip_count",
                table: "contract_billing_snapshot_calendar_days");

            migrationBuilder.AddColumn<long[]>(
                name: "trip_ids",
                table: "contract_billing_snapshot_calendar_days",
                type: "bigint[]",
                nullable: false,
                defaultValue: new long[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "trip_ids",
                table: "contract_billing_snapshot_calendar_days");

            migrationBuilder.AddColumn<int>(
                name: "trip_count",
                table: "contract_billing_snapshot_calendar_days",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
