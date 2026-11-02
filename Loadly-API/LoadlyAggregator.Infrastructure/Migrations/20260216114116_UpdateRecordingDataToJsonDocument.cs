using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ChangeRecordingsForeignKeyToNoAction : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_recordings_trips_recordable_id",
            table: "recordings");

        migrationBuilder.AddForeignKey(
            name: "FK_recordings_trips_recordable_id",
            table: "recordings",
            column: "recordable_id",
            principalTable: "trips",
            principalColumn: "id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_recordings_trips_recordable_id",
            table: "recordings");

        migrationBuilder.AddForeignKey(
            name: "FK_recordings_trips_recordable_id",
            table: "recordings",
            column: "recordable_id",
            principalTable: "trips",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
