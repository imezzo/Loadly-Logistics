using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddVehiclesTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "vehicles",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                is_head = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                plate_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                plate_letters = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                make_year = table.Column<int>(type: "integer", nullable: true),
                insurance_id = table.Column<long>(type: "bigint", nullable: true),
                purchase_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                initial_odometer_reading = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                vin_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                color_id = table.Column<long>(type: "bigint", nullable: true),
                capacity_unit_id = table.Column<long>(type: "bigint", nullable: true),
                tank_capacity = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                vehicle_type_settings = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                capacity = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_vehicles", x => x.id);
                table.ForeignKey(
                    name: "FK_vehicles_colors_color_id",
                    column: x => x.color_id,
                    principalTable: "colors",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_vehicles_documents_insurance_id",
                    column: x => x.insurance_id,
                    principalTable: "documents",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_vehicles_vehicles_capacity_units_capacity_unit_id",
                    column: x => x.capacity_unit_id,
                    principalTable: "vehicles_capacity_units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_capacity_unit_id",
            table: "vehicles",
            column: "capacity_unit_id");

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_color_id",
            table: "vehicles",
            column: "color_id");

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_insurance_id",
            table: "vehicles",
            column: "insurance_id");

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_plate_no",
            table: "vehicles",
            column: "plate_no");

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_status",
            table: "vehicles",
            column: "status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "vehicles");
    }
}
