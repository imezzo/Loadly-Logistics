using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecordingsLatestEventIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_recordings_recordable_id_recordable_type",
                table: "recordings");

            migrationBuilder.CreateIndex(
                name: "IX_recordings_recordable_id_recordable_type_created_at",
                table: "recordings",
                columns: new[] { "recordable_id", "recordable_type", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_recordings_recordable_id_recordable_type_created_at",
                table: "recordings");

            migrationBuilder.CreateIndex(
                name: "IX_recordings_recordable_id_recordable_type",
                table: "recordings",
                columns: new[] { "recordable_id", "recordable_type" });
        }
    }
}
