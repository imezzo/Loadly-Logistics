using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations.LogsDb
{
    /// <inheritdoc />
    public partial class AddOdometerScanLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "odometer_scan_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    model_id = table.Column<long>(type: "bigint", nullable: true),
                    model_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    trip_id = table.Column<long>(type: "bigint", nullable: true),
                    scan_context = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    image_path = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    suggested_reading = table.Column<int>(type: "integer", nullable: true),
                    unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_digital = table.Column<bool>(type: "boolean", nullable: true),
                    reading_quality = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_screen = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    total_tokens = table.Column<int>(type: "integer", nullable: true),
                    latency_ms = table.Column<int>(type: "integer", nullable: true),
                    raw_response = table.Column<string>(type: "text", nullable: true),
                    correlation_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    previous_scan_log_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_odometer_scan_logs", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_odometer_scan_logs_correlation_id",
                table: "odometer_scan_logs",
                column: "correlation_id");

            migrationBuilder.CreateIndex(
                name: "IX_odometer_scan_logs_created_at",
                table: "odometer_scan_logs",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_odometer_scan_logs_model_type_model_id",
                table: "odometer_scan_logs",
                columns: new[] { "model_type", "model_id" });

            migrationBuilder.CreateIndex(
                name: "IX_odometer_scan_logs_status",
                table: "odometer_scan_logs",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_odometer_scan_logs_trip_id_created_at",
                table: "odometer_scan_logs",
                columns: new[] { "trip_id", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "odometer_scan_logs");
        }
    }
}
