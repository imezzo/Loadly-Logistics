using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddCompanyIdToRoles : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_roles_name_guard_name",
            table: "roles");

        migrationBuilder.AddColumn<long>(
            name: "company_id",
            table: "roles",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_roles_company_id",
            table: "roles",
            column: "company_id");

        migrationBuilder.CreateIndex(
            name: "IX_roles_name_guard_name",
            table: "roles",
            columns: new[] { "name", "guard_name" },
            unique: true,
            filter: "\"company_id\" IS NULL");

        migrationBuilder.CreateIndex(
            name: "IX_roles_name_guard_name_company_id",
            table: "roles",
            columns: new[] { "name", "guard_name", "company_id" },
            unique: true,
            filter: "\"company_id\" IS NOT NULL");

        migrationBuilder.AddForeignKey(
            name: "FK_roles_companies_company_id",
            table: "roles",
            column: "company_id",
            principalTable: "companies",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_roles_companies_company_id",
            table: "roles");

        migrationBuilder.DropIndex(
            name: "IX_roles_company_id",
            table: "roles");

        migrationBuilder.DropIndex(
            name: "IX_roles_name_guard_name",
            table: "roles");

        migrationBuilder.DropIndex(
            name: "IX_roles_name_guard_name_company_id",
            table: "roles");

        migrationBuilder.DropColumn(
            name: "company_id",
            table: "roles");

        migrationBuilder.CreateIndex(
            name: "IX_roles_name_guard_name",
            table: "roles",
            columns: new[] { "name", "guard_name" },
            unique: true);
    }
}
