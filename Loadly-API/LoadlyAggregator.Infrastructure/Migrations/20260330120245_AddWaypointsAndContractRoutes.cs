using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWaypointsAndContractRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "waypoints",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    google_place_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    latitude = table.Column<decimal>(type: "numeric(10,8)", nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(11,8)", nullable: true),
                    address_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    added_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_waypoints", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contract_destinations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    contract_id = table.Column<long>(type: "bigint", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    start_waypoint_id = table.Column<long>(type: "bigint", nullable: false),
                    end_waypoint_id = table.Column<long>(type: "bigint", nullable: false),
                    name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    expected_kms = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    added_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract_destinations", x => x.id);
                    table.ForeignKey(
                        name: "FK_contract_destinations_contracts_contract_id",
                        column: x => x.contract_id,
                        principalTable: "contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contract_destinations_waypoints_end_waypoint_id",
                        column: x => x.end_waypoint_id,
                        principalTable: "waypoints",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contract_destinations_waypoints_start_waypoint_id",
                        column: x => x.start_waypoint_id,
                        principalTable: "waypoints",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contract_destinations_contract_id",
                table: "contract_destinations",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_destinations_contract_id_start_waypoint_id_end_way~",
                table: "contract_destinations",
                columns: new[] { "contract_id", "start_waypoint_id", "end_waypoint_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contract_destinations_end_waypoint_id",
                table: "contract_destinations",
                column: "end_waypoint_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_destinations_start_waypoint_id",
                table: "contract_destinations",
                column: "start_waypoint_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contract_destinations");

            migrationBuilder.DropTable(
                name: "waypoints");
        }
    }
}
