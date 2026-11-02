using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeCompanyDetailRegistrationNumberOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_company_details_registration_number",
                table: "company_details");

            migrationBuilder.AlterColumn<string>(
                name: "registration_number",
                table: "company_details",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_company_details_registration_number",
                table: "company_details",
                column: "registration_number",
                unique: true,
                filter: "\"registration_number\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_company_details_registration_number",
                table: "company_details");

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

            migrationBuilder.CreateIndex(
                name: "IX_company_details_registration_number",
                table: "company_details",
                column: "registration_number",
                unique: true);
        }
    }
}
