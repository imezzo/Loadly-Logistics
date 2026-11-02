using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class MoveWorkingStatusToDriversTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "working_status",
            table: "drivers",
            type: "integer",
            nullable: true,
            defaultValue: 1);

        migrationBuilder.Sql(@"
                UPDATE drivers d
                SET working_status = dd.working_status
                FROM driver_details dd
                WHERE dd.driver_id = d.id AND dd.working_status IS NOT NULL
            ");

        migrationBuilder.DropColumn(
            name: "working_status",
            table: "driver_details");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "working_status",
            table: "driver_details",
            type: "integer",
            nullable: true,
            defaultValue: 1);

        migrationBuilder.Sql(@"
                UPDATE driver_details dd
                SET working_status = d.working_status
                FROM drivers d
                WHERE dd.driver_id = d.id AND d.working_status IS NOT NULL
            ");

        migrationBuilder.DropColumn(
            name: "working_status",
            table: "drivers");
    }
}
