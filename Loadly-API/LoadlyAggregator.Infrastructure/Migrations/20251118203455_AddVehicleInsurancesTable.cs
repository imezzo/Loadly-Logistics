using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddVehicleInsurancesTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "vehicles_insurance",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                policy_no = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                insurance_provider = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_vehicles_insurance", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_insurance_end_date",
            table: "vehicles_insurance",
            column: "end_date");

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_insurance_policy_no",
            table: "vehicles_insurance",
            column: "policy_no");

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_insurance_start_date",
            table: "vehicles_insurance",
            column: "start_date");

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_insurance_status",
            table: "vehicles_insurance",
            column: "status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "vehicles_insurance");
    }
}
