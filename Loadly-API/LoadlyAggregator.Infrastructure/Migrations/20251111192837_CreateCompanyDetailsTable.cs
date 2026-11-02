using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateCompanyDetailsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "company_details",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                company_id = table.Column<long>(type: "bigint", nullable: false),
                name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                registration_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                tax_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                website_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                industry_type_id = table.Column<long>(type: "bigint", nullable: true),
                company_size = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                service_requirements_ids = table.Column<string>(type: "text", nullable: true),
                business_hour_id = table.Column<long>(type: "bigint", nullable: true),
                communications_method_id = table.Column<long>(type: "bigint", nullable: true),
                verification_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                qr_code = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_company_details", x => x.id);
                table.ForeignKey(
                    name: "FK_company_details_companies_company_id",
                    column: x => x.company_id,
                    principalTable: "companies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_company_details_company_id",
            table: "company_details",
            column: "company_id");

        migrationBuilder.CreateIndex(
            name: "IX_company_details_verification_status",
            table: "company_details",
            column: "verification_status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "company_details");
    }
}
