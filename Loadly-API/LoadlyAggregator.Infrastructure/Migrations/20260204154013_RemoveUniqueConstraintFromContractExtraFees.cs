using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveUniqueConstraintFromContractExtraFees : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_contract_extra_fees_contract_id_extra_fee_type_id",
            table: "contract_extra_fees");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_contract_extra_fees_contract_id_extra_fee_type_id",
            table: "contract_extra_fees",
            columns: new[] { "contract_id", "extra_fee_type_id" },
            unique: true);
    }
}
