using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniquePhoneIndexOnLeads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_leads_phone",
                table: "leads");

            migrationBuilder.CreateIndex(
                name: "IX_leads_phone",
                table: "leads",
                column: "phone",
                unique: true,
                filter: "\"deleted_at\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_leads_phone",
                table: "leads");

            migrationBuilder.CreateIndex(
                name: "IX_leads_phone",
                table: "leads",
                column: "phone");
        }
    }
}
