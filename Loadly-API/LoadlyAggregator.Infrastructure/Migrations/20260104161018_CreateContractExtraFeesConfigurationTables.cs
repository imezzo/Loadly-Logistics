using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateContractExtraFeesConfigurationTables : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "extra_fee_types",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_extra_fee_types", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "contract_extra_fees",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                contract_id = table.Column<long>(type: "bigint", nullable: false),
                extra_fee_type_id = table.Column<long>(type: "bigint", nullable: false),
                calculation_option = table.Column<int>(type: "integer", nullable: true),
                value = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                threshold = table.Column<int>(type: "integer", nullable: true),
                notes = table.Column<string>(type: "text", nullable: true),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_contract_extra_fees", x => x.id);
                table.ForeignKey(
                    name: "FK_contract_extra_fees_contracts_contract_id",
                    column: x => x.contract_id,
                    principalTable: "contracts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_contract_extra_fees_extra_fee_types_extra_fee_type_id",
                    column: x => x.extra_fee_type_id,
                    principalTable: "extra_fee_types",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_contract_extra_fees_contract_id",
            table: "contract_extra_fees",
            column: "contract_id");

        migrationBuilder.CreateIndex(
            name: "IX_contract_extra_fees_contract_id_extra_fee_type_id",
            table: "contract_extra_fees",
            columns: new[] { "contract_id", "extra_fee_type_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_contract_extra_fees_extra_fee_type_id",
            table: "contract_extra_fees",
            column: "extra_fee_type_id");

        migrationBuilder.CreateIndex(
            name: "IX_extra_fee_types_code",
            table: "extra_fee_types",
            column: "code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_extra_fee_types_name_ar",
            table: "extra_fee_types",
            column: "name_ar",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_extra_fee_types_name_en",
            table: "extra_fee_types",
            column: "name_en",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_extra_fee_types_status",
            table: "extra_fee_types",
            column: "status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "contract_extra_fees");

        migrationBuilder.DropTable(
            name: "extra_fee_types");
    }
}
