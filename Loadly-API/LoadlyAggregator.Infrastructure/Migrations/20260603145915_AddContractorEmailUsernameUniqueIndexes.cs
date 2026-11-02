using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractorEmailUsernameUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_contractors_email",
                table: "contractors",
                column: "email",
                unique: true,
                filter: "\"email\" IS NOT NULL AND \"email\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_contractors_username",
                table: "contractors",
                column: "username",
                unique: true,
                filter: "\"username\" IS NOT NULL AND \"username\" <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_contractors_email",
                table: "contractors");

            migrationBuilder.DropIndex(
                name: "IX_contractors_username",
                table: "contractors");
        }
    }
}
