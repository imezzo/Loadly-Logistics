using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractSlotsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contract_slots",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    assigned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    contract_id = table.Column<long>(type: "bigint", nullable: false),
                    vehicle_type_id = table.Column<long>(type: "bigint", nullable: false),
                    vehicle_id = table.Column<long>(type: "bigint", nullable: true),
                    consumption_rate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    trips_per_month = table.Column<int>(type: "integer", nullable: true),
                    trips_per_day = table.Column<int>(type: "integer", nullable: true),
                    max_kms = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    monthly_rental = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    daily_value = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    over_consumption = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    min_billable_weight_tons = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    max_payload_tons = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    overweight_price_per_ton = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    added_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract_slots", x => x.id);
                    table.ForeignKey(
                        name: "FK_contract_slots_contracts_contract_id",
                        column: x => x.contract_id,
                        principalTable: "contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contract_slots_vehicle_types_vehicle_type_id",
                        column: x => x.vehicle_type_id,
                        principalTable: "vehicle_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contract_slots_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });



            migrationBuilder.CreateIndex(
                name: "IX_contract_slots_contract_id",
                table: "contract_slots",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_slots_vehicle_id",
                table: "contract_slots",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_slots_vehicle_type_id",
                table: "contract_slots",
                column: "vehicle_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contract_slots");
        }
    }
}
