using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDocumentNameArUniqueConstraintToIncludeEntityTypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_documents_name_ar",
                table: "documents");

            migrationBuilder.CreateIndex(
                name: "IX_documents_entity_id_name_ar",
                table: "documents",
                columns: new[] { "entity_id", "name_ar" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_documents_entity_id_name_ar",
                table: "documents");

            migrationBuilder.CreateIndex(
                name: "IX_documents_name_ar",
                table: "documents",
                column: "name_ar",
                unique: true);
        }
    }
}
