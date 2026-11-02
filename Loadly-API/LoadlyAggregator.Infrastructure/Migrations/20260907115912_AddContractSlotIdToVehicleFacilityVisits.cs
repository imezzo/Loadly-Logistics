using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractSlotIdToVehicleFacilityVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "contract_slot_id",
                table: "vehicle_facility_visits",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_facility_visits_contract_slot_id",
                table: "vehicle_facility_visits",
                column: "contract_slot_id");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicle_facility_visits_contract_slots_contract_slot_id",
                table: "vehicle_facility_visits",
                column: "contract_slot_id",
                principalTable: "contract_slots",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicle_facility_visits_contract_slots_contract_slot_id",
                table: "vehicle_facility_visits");

            migrationBuilder.DropIndex(
                name: "IX_vehicle_facility_visits_contract_slot_id",
                table: "vehicle_facility_visits");

            migrationBuilder.DropColumn(
                name: "contract_slot_id",
                table: "vehicle_facility_visits");
        }
    }
}
