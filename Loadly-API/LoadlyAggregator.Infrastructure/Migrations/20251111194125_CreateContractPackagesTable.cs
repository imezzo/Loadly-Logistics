using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateContractPackagesTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "contract_packages",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                contract_id = table.Column<long>(type: "bigint", nullable: false),
                package_id = table.Column<long>(type: "bigint", nullable: false),
                discount_id = table.Column<long>(type: "bigint", nullable: true),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                calculation_method = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                km_consumption_alert = table.Column<string>(type: "text", nullable: false),
                expiry_date_alert = table.Column<int>(type: "integer", nullable: true),
                auto_renew = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                rollover = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_contract_packages", x => x.id);
                table.ForeignKey(
                    name: "FK_contract_packages_company_contracts_contract_id",
                    column: x => x.contract_id,
                    principalTable: "company_contracts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_contract_packages_packages_package_id",
                    column: x => x.package_id,
                    principalTable: "packages",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_contract_packages_contract_id",
            table: "contract_packages",
            column: "contract_id");

        migrationBuilder.CreateIndex(
            name: "IX_contract_packages_end_date",
            table: "contract_packages",
            column: "end_date");

        migrationBuilder.CreateIndex(
            name: "IX_contract_packages_package_id",
            table: "contract_packages",
            column: "package_id");

        migrationBuilder.CreateIndex(
            name: "IX_contract_packages_start_date",
            table: "contract_packages",
            column: "start_date");

        migrationBuilder.CreateIndex(
            name: "IX_contract_packages_status",
            table: "contract_packages",
            column: "status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "contract_packages");
    }
}
