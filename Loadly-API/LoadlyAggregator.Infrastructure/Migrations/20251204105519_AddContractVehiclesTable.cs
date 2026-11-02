using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddContractVehiclesTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "contract_vehicles",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                contract_id = table.Column<long>(type: "bigint", nullable: false),
                vehicle_type_id = table.Column<long>(type: "bigint", nullable: false),
                consumption_rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                daily_value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                over_consumption = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_contract_vehicles", x => x.id);
                table.ForeignKey(
                    name: "FK_contract_vehicles_contracts_contract_id",
                    column: x => x.contract_id,
                    principalTable: "contracts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_contract_vehicles_vehicle_types_vehicle_type_id",
                    column: x => x.vehicle_type_id,
                    principalTable: "vehicle_types",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_contract_vehicles_contract_id",
            table: "contract_vehicles",
            column: "contract_id");

        migrationBuilder.CreateIndex(
            name: "IX_contract_vehicles_vehicle_type_id",
            table: "contract_vehicles",
            column: "vehicle_type_id");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "contract_vehicles");
    }
}
