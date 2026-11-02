using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateTripContractDestinationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trip_contract_destinations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    trip_id = table.Column<long>(type: "bigint", nullable: false),
                    destination_id = table.Column<long>(type: "bigint", nullable: false),
                    return_destination_id = table.Column<long>(type: "bigint", nullable: true),
                    outbound_distance_km = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    return_distance_km = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    added_by = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_contract_destinations", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_contract_destinations_admins_added_by",
                        column: x => x.added_by,
                        principalTable: "admins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trip_contract_destinations_contract_destinations_destinatio~",
                        column: x => x.destination_id,
                        principalTable: "contract_destinations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trip_contract_destinations_contract_destinations_return_des~",
                        column: x => x.return_destination_id,
                        principalTable: "contract_destinations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trip_contract_destinations_trips_trip_id",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trip_contract_destinations_added_by",
                table: "trip_contract_destinations",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "IX_trip_contract_destinations_destination_id",
                table: "trip_contract_destinations",
                column: "destination_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_contract_destinations_return_destination_id",
                table: "trip_contract_destinations",
                column: "return_destination_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_contract_destinations_trip_id",
                table: "trip_contract_destinations",
                column: "trip_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trip_contract_destinations");
        }
    }
}
