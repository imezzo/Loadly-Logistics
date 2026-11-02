using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreatePackagesTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "packages",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                desc_en = table.Column<string>(type: "text", nullable: true),
                desc_ar = table.Column<string>(type: "text", nullable: true),
                period_type = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                included_km = table.Column<int>(type: "integer", nullable: true),
                overage_rate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                rollover_policy = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                max_rollover = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                sub_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                vat_id = table.Column<long>(type: "bigint", nullable: true),
                discount_id = table.Column<long>(type: "bigint", nullable: true),
                grand_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                km_unit_price = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                recommended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_packages", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_packages_period_type",
            table: "packages",
            column: "period_type");

        migrationBuilder.CreateIndex(
            name: "IX_packages_recommended",
            table: "packages",
            column: "recommended");

        migrationBuilder.CreateIndex(
            name: "IX_packages_status",
            table: "packages",
            column: "status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "packages");
    }
}
