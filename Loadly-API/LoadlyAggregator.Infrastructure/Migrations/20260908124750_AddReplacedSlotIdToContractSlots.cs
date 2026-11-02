using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReplacedSlotIdToContractSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "replaced_slot_id",
                table: "contract_slots",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_contract_slots_replaced_slot_id",
                table: "contract_slots",
                column: "replaced_slot_id");

            migrationBuilder.AddForeignKey(
                name: "FK_contract_slots_contract_slots_replaced_slot_id",
                table: "contract_slots",
                column: "replaced_slot_id",
                principalTable: "contract_slots",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contract_slots_contract_slots_replaced_slot_id",
                table: "contract_slots");

            migrationBuilder.DropIndex(
                name: "IX_contract_slots_replaced_slot_id",
                table: "contract_slots");

            migrationBuilder.DropColumn(
                name: "replaced_slot_id",
                table: "contract_slots");
        }
    }
}
