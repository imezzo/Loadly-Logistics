using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractBillingSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "contract_billing_snapshot_id",
                table: "invoices",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "slot_charges_calculated_at",
                table: "invoices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "slot_charges_sub_total",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "slot_extra_kms_amount",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "slot_extra_kms_count",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "slot_extra_trips_amount",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "slot_extra_trips_count",
                table: "invoices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "slot_extra_working_days_amount",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "slot_extra_working_days_count",
                table: "invoices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "slot_working_days_amount",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "slot_working_days_count",
                table: "invoices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "contract_billing_snapshots",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    contract_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    period_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    period_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    contract_type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    engine_version = table.Column<int>(type: "integer", nullable: false),
                    invoice_id = table.Column<long>(type: "bigint", nullable: true),
                    finalized_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sub_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    working_days_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    extra_working_days_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    extra_kms_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    extra_trips_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    working_days_count = table.Column<int>(type: "integer", nullable: false),
                    extra_working_days_count = table.Column<int>(type: "integer", nullable: false),
                    extra_trips_count = table.Column<int>(type: "integer", nullable: false),
                    extra_kms_count = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    km_pool_json = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract_billing_snapshots", x => x.id);
                    table.ForeignKey(
                        name: "FK_contract_billing_snapshots_contracts_contract_id",
                        column: x => x.contract_id,
                        principalTable: "contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contract_billing_snapshot_calendar_days",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    snapshot_id = table.Column<long>(type: "bigint", nullable: false),
                    contract_slot_id = table.Column<long>(type: "bigint", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    disposition = table.Column<int>(type: "integer", nullable: false),
                    disposition_source = table.Column<int>(type: "integer", nullable: false),
                    trip_count = table.Column<int>(type: "integer", nullable: false),
                    working_day_index = table.Column<int>(type: "integer", nullable: true),
                    is_extra = table.Column<bool>(type: "boolean", nullable: false),
                    facility_type = table.Column<int>(type: "integer", nullable: true),
                    facility_id = table.Column<long>(type: "bigint", nullable: true),
                    override_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract_billing_snapshot_calendar_days", x => x.id);
                    table.ForeignKey(
                        name: "FK_contract_billing_snapshot_calendar_days_contract_billing_sn~",
                        column: x => x.snapshot_id,
                        principalTable: "contract_billing_snapshots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contract_billing_snapshot_calendar_days_contract_slots_cont~",
                        column: x => x.contract_slot_id,
                        principalTable: "contract_slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contract_billing_snapshot_calendar_days_vehicle_facilities_~",
                        column: x => x.facility_id,
                        principalTable: "vehicle_facilities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contract_billing_snapshot_charge_lines",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    snapshot_id = table.Column<long>(type: "bigint", nullable: false),
                    contract_slot_id = table.Column<long>(type: "bigint", nullable: true),
                    date = table.Column<DateOnly>(type: "date", nullable: true),
                    charge_kind = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract_billing_snapshot_charge_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_contract_billing_snapshot_charge_lines_contract_billing_sna~",
                        column: x => x.snapshot_id,
                        principalTable: "contract_billing_snapshots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contract_billing_snapshot_charge_lines_contract_slots_contr~",
                        column: x => x.contract_slot_id,
                        principalTable: "contract_slots",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "contract_billing_snapshot_km_evidence",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    snapshot_id = table.Column<long>(type: "bigint", nullable: false),
                    contract_slot_id = table.Column<long>(type: "bigint", nullable: true),
                    is_pooled = table.Column<bool>(type: "boolean", nullable: false),
                    consumption = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    max_kms = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    overage_km = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    over_consumption_rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract_billing_snapshot_km_evidence", x => x.id);
                    table.ForeignKey(
                        name: "FK_contract_billing_snapshot_km_evidence_contract_billing_snap~",
                        column: x => x.snapshot_id,
                        principalTable: "contract_billing_snapshots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contract_billing_snapshot_km_evidence_contract_slots_contra~",
                        column: x => x.contract_slot_id,
                        principalTable: "contract_slots",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_invoices_contract_billing_snapshot_id",
                table: "invoices",
                column: "contract_billing_snapshot_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_billing_snapshot_calendar_days_contract_slot_id",
                table: "contract_billing_snapshot_calendar_days",
                column: "contract_slot_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_billing_snapshot_calendar_days_facility_id",
                table: "contract_billing_snapshot_calendar_days",
                column: "facility_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_billing_snapshot_calendar_days_snapshot_id_contrac~",
                table: "contract_billing_snapshot_calendar_days",
                columns: new[] { "snapshot_id", "contract_slot_id", "date" },
                unique: true,
                filter: "\"deleted_at\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_contract_billing_snapshot_charge_lines_contract_slot_id",
                table: "contract_billing_snapshot_charge_lines",
                column: "contract_slot_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_billing_snapshot_charge_lines_snapshot_id",
                table: "contract_billing_snapshot_charge_lines",
                column: "snapshot_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_billing_snapshot_km_evidence_contract_slot_id",
                table: "contract_billing_snapshot_km_evidence",
                column: "contract_slot_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_billing_snapshot_km_evidence_snapshot_id",
                table: "contract_billing_snapshot_km_evidence",
                column: "snapshot_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_billing_snapshots_contract_id_period_start_period_~",
                table: "contract_billing_snapshots",
                columns: new[] { "contract_id", "period_start", "period_end", "status" });

            migrationBuilder.AddForeignKey(
                name: "FK_invoices_contract_billing_snapshots_contract_billing_snapsh~",
                table: "invoices",
                column: "contract_billing_snapshot_id",
                principalTable: "contract_billing_snapshots",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoices_contract_billing_snapshots_contract_billing_snapsh~",
                table: "invoices");

            migrationBuilder.DropTable(
                name: "contract_billing_snapshot_calendar_days");

            migrationBuilder.DropTable(
                name: "contract_billing_snapshot_charge_lines");

            migrationBuilder.DropTable(
                name: "contract_billing_snapshot_km_evidence");

            migrationBuilder.DropTable(
                name: "contract_billing_snapshots");

            migrationBuilder.DropIndex(
                name: "IX_invoices_contract_billing_snapshot_id",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "contract_billing_snapshot_id",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "slot_charges_calculated_at",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "slot_charges_sub_total",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "slot_extra_kms_amount",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "slot_extra_kms_count",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "slot_extra_trips_amount",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "slot_extra_trips_count",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "slot_extra_working_days_amount",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "slot_extra_working_days_count",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "slot_working_days_amount",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "slot_working_days_count",
                table: "invoices");
        }
    }
}
