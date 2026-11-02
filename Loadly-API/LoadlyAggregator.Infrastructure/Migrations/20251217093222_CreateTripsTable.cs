using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateTripsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "trips",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                contract_id = table.Column<long>(type: "bigint", nullable: false),
                vehicle_id = table.Column<long>(type: "bigint", nullable: false),
                start_location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                end_location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                fuel_quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_trips", x => x.id);
                table.ForeignKey(
                    name: "FK_trips_contracts_contract_id",
                    column: x => x.contract_id,
                    principalTable: "contracts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_trips_vehicles_vehicle_id",
                    column: x => x.vehicle_id,
                    principalTable: "vehicles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_trips_contract_id",
            table: "trips",
            column: "contract_id");

        migrationBuilder.CreateIndex(
            name: "IX_trips_end_date",
            table: "trips",
            column: "end_date");

        migrationBuilder.CreateIndex(
            name: "IX_trips_start_date",
            table: "trips",
            column: "start_date");

        migrationBuilder.CreateIndex(
            name: "IX_trips_vehicle_id",
            table: "trips",
            column: "vehicle_id");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "trips");
    }
}
