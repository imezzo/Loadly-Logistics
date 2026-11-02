using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateContractVehiclesTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "contract_vehicles",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                vehicle_id = table.Column<long>(type: "bigint", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                assigned_from = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                assigned_to = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_contract_vehicles", x => x.id);
                table.ForeignKey(
                    name: "FK_contract_vehicles_vehicles_vehicle_id",
                    column: x => x.vehicle_id,
                    principalTable: "vehicles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_contract_vehicles_status",
            table: "contract_vehicles",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "IX_contract_vehicles_vehicle_id",
            table: "contract_vehicles",
            column: "vehicle_id");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "contract_vehicles");
    }
}
