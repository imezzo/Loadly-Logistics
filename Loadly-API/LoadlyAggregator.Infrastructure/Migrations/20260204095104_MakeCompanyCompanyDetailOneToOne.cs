using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class MakeCompanyCompanyDetailOneToOne : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_company_details_company_id",
            table: "company_details");

        migrationBuilder.CreateIndex(
            name: "IX_company_details_company_id",
            table: "company_details",
            column: "company_id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_company_details_company_id",
            table: "company_details");

        migrationBuilder.CreateIndex(
            name: "IX_company_details_company_id",
            table: "company_details",
            column: "company_id");
    }
}
