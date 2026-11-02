using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateContractFuelTypesTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "contract_fuel_types",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                contract_id = table.Column<long>(type: "bigint", nullable: false),
                fuel_type_id = table.Column<long>(type: "bigint", nullable: false),
                unit_price = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_contract_fuel_types", x => x.id);
                table.ForeignKey(
                    name: "FK_contract_fuel_types_contracts_contract_id",
                    column: x => x.contract_id,
                    principalTable: "contracts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_contract_fuel_types_fuel_types_fuel_type_id",
                    column: x => x.fuel_type_id,
                    principalTable: "fuel_types",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_contract_fuel_types_contract_id",
            table: "contract_fuel_types",
            column: "contract_id");

        migrationBuilder.CreateIndex(
            name: "IX_contract_fuel_types_fuel_type_id",
            table: "contract_fuel_types",
            column: "fuel_type_id");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "contract_fuel_types");
    }
}
