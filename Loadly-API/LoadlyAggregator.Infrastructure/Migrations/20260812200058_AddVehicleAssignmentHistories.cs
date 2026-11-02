using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleAssignmentHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vehicle_assignment_histories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vehicle_id = table.Column<long>(type: "bigint", nullable: false),
                    prev_tail_vehicle_id = table.Column<long>(type: "bigint", nullable: true),
                    new_tail_vehicle_id = table.Column<long>(type: "bigint", nullable: true),
                    prev_driver_id = table.Column<long>(type: "bigint", nullable: true),
                    new_driver_id = table.Column<long>(type: "bigint", nullable: true),
                    added_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle_assignment_histories", x => x.id);
                    table.ForeignKey(
                        name: "FK_vehicle_assignment_histories_admins_added_by",
                        column: x => x.added_by,
                        principalTable: "admins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_vehicle_assignment_histories_drivers_new_driver_id",
                        column: x => x.new_driver_id,
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_vehicle_assignment_histories_drivers_prev_driver_id",
                        column: x => x.prev_driver_id,
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_vehicle_assignment_histories_vehicles_new_tail_vehicle_id",
                        column: x => x.new_tail_vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_vehicle_assignment_histories_vehicles_prev_tail_vehicle_id",
                        column: x => x.prev_tail_vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_vehicle_assignment_histories_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_assignment_histories_added_by",
                table: "vehicle_assignment_histories",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_assignment_histories_created_at",
                table: "vehicle_assignment_histories",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_assignment_histories_new_driver_id",
                table: "vehicle_assignment_histories",
                column: "new_driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_assignment_histories_new_tail_vehicle_id",
                table: "vehicle_assignment_histories",
                column: "new_tail_vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_assignment_histories_prev_driver_id",
                table: "vehicle_assignment_histories",
                column: "prev_driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_assignment_histories_prev_tail_vehicle_id",
                table: "vehicle_assignment_histories",
                column: "prev_tail_vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_assignment_histories_vehicle_id",
                table: "vehicle_assignment_histories",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_assignment_histories_vehicle_id_created_at",
                table: "vehicle_assignment_histories",
                columns: new[] { "vehicle_id", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vehicle_assignment_histories");
        }
    }
}
