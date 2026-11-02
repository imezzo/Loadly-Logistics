using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateVehicleChangeRequestsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "vehicle_change_requests",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                driver_id = table.Column<long>(type: "bigint", nullable: false),
                requested_vehicles = table.Column<string>(type: "text", nullable: true),
                reason_category = table.Column<int>(type: "integer", nullable: true),
                explanation = table.Column<string>(type: "text", nullable: true),
                prefered_effective_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                request_priority = table.Column<int>(type: "integer", nullable: true),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                updated_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_vehicle_change_requests", x => x.id);
                table.ForeignKey(
                    name: "FK_vehicle_change_requests_drivers_driver_id",
                    column: x => x.driver_id,
                    principalTable: "drivers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_vehicle_change_requests_driver_id",
            table: "vehicle_change_requests",
            column: "driver_id");

        migrationBuilder.CreateIndex(
            name: "IX_vehicle_change_requests_status",
            table: "vehicle_change_requests",
            column: "status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "vehicle_change_requests");
    }
}
