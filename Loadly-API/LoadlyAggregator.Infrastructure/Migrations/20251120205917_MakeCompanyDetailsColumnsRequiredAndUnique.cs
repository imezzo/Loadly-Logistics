using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class MakeCompanyDetailsColumnsRequiredAndUnique : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "website_url",
            table: "company_details",
            type: "character varying(500)",
            maxLength: 500,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "character varying(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "registration_number",
            table: "company_details",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "character varying(100)",
            oldMaxLength: 100,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "name_ar",
            table: "company_details",
            type: "character varying(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "character varying(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_company_details_name_ar",
            table: "company_details",
            column: "name_ar",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_company_details_registration_number",
            table: "company_details",
            column: "registration_number",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_company_details_website_url",
            table: "company_details",
            column: "website_url",
            unique: true);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_company_details_name_ar",
            table: "company_details");

        migrationBuilder.DropIndex(
            name: "IX_company_details_registration_number",
            table: "company_details");

        migrationBuilder.DropIndex(
            name: "IX_company_details_website_url",
            table: "company_details");

        migrationBuilder.AlterColumn<string>(
            name: "website_url",
            table: "company_details",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(500)",
            oldMaxLength: 500);

        migrationBuilder.AlterColumn<string>(
            name: "registration_number",
            table: "company_details",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(100)",
            oldMaxLength: 100);

        migrationBuilder.AlterColumn<string>(
            name: "name_ar",
            table: "company_details",
            type: "character varying(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(255)",
            oldMaxLength: 255);
    }
}
