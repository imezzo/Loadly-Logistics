using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractBillingSnapshotCommercialOverrides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contract_billing_snapshot_commercial_overrides",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    snapshot_id = table.Column<long>(type: "bigint", nullable: false),
                    seat_root_slot_id = table.Column<long>(type: "bigint", nullable: false),
                    effective_start = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_end = table.Column<DateOnly>(type: "date", nullable: false),
                    daily_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    extra_working_day_fees = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    over_consumption = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    override_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    added_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract_billing_snapshot_commercial_overrides", x => x.id);
                    table.ForeignKey(
                        name: "FK_contract_billing_snapshot_commercial_overrides_contract_bil~",
                        column: x => x.snapshot_id,
                        principalTable: "contract_billing_snapshots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contract_billing_snapshot_commercial_overrides_contract_slo~",
                        column: x => x.seat_root_slot_id,
                        principalTable: "contract_slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contract_billing_snapshot_commercial_overrides_seat_root_sl~",
                table: "contract_billing_snapshot_commercial_overrides",
                column: "seat_root_slot_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_billing_snapshot_commercial_overrides_snapshot_id_~",
                table: "contract_billing_snapshot_commercial_overrides",
                columns: new[] { "snapshot_id", "seat_root_slot_id" },
                unique: true,
                filter: "\"deleted_at\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contract_billing_snapshot_commercial_overrides");
        }
    }
}
