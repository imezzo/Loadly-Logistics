using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class DropCompanyContractsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "company_contracts");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "company_contracts",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                company_id = table.Column<long>(type: "bigint", nullable: false),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                terms_and_conditions = table.Column<string>(type: "text", nullable: true),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                vehicle_type_ids = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_company_contracts", x => x.id);
                table.ForeignKey(
                    name: "FK_company_contracts_companies_company_id",
                    column: x => x.company_id,
                    principalTable: "companies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_company_contracts_company_id",
            table: "company_contracts",
            column: "company_id");

        migrationBuilder.CreateIndex(
            name: "IX_company_contracts_end_date",
            table: "company_contracts",
            column: "end_date");

        migrationBuilder.CreateIndex(
            name: "IX_company_contracts_start_date",
            table: "company_contracts",
            column: "start_date");

        migrationBuilder.CreateIndex(
            name: "IX_company_contracts_status",
            table: "company_contracts",
            column: "status");
    }
}
