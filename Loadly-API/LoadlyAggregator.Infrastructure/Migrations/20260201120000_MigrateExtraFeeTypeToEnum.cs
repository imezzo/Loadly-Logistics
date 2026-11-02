using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

public partial class MigrateExtraFeeTypeToEnum : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_contract_extra_fees_extra_fee_types_extra_fee_type_id",
            table: "contract_extra_fees");

        migrationBuilder.DropTable(
            name: "extra_fee_types");

        migrationBuilder.AlterColumn<int>(
            name: "extra_fee_type_id",
            table: "contract_extra_fees",
            type: "integer",
            nullable: false,
            oldClrType: typeof(long),
            oldType: "bigint");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<long>(
            name: "extra_fee_type_id",
            table: "contract_extra_fees",
            type: "bigint",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.CreateTable(
            name: "extra_fee_types",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false),
                code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_extra_fee_types", x => x.id);
            });

        migrationBuilder.AddForeignKey(
            name: "FK_contract_extra_fees_extra_fee_types_extra_fee_type_id",
            table: "contract_extra_fees",
            column: "extra_fee_type_id",
            principalTable: "extra_fee_types",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }
}
