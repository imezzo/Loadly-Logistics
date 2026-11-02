using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCalendarDayDispositionChangedAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "changed_at",
                table: "contract_billing_snapshot_calendar_days",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "changed_by",
                table: "contract_billing_snapshot_calendar_days",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "changed_at",
                table: "contract_billing_snapshot_calendar_days");

            migrationBuilder.DropColumn(
                name: "changed_by",
                table: "contract_billing_snapshot_calendar_days");
        }
    }
}
