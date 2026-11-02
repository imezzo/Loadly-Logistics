using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddContractsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "contracts",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                company_id = table.Column<long>(type: "bigint", nullable: false),
                price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                billing_cycle = table.Column<int>(type: "integer", nullable: false),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                consumption_type = table.Column<int>(type: "integer", nullable: false),
                trips_per_month = table.Column<int>(type: "integer", nullable: true),
                trips_per_day = table.Column<int>(type: "integer", nullable: true),
                max_kms = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                status = table.Column<int>(type: "integer", nullable: true),
                terms_and_conditions = table.Column<string>(type: "text", nullable: true),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_contracts", x => x.id);
                table.ForeignKey(
                    name: "FK_contracts_companies_company_id",
                    column: x => x.company_id,
                    principalTable: "companies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_contracts_company_id",
            table: "contracts",
            column: "company_id");

        migrationBuilder.CreateIndex(
            name: "IX_contracts_name_ar",
            table: "contracts",
            column: "name_ar",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_contracts_name_en",
            table: "contracts",
            column: "name_en",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_contracts_status",
            table: "contracts",
            column: "status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "contracts");
    }
}
