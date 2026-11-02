using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceToDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "source",
                table: "documents",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_documents_entity_id_source",
                table: "documents",
                columns: new[] { "entity_id", "source" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_documents_entity_id_source",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "source",
                table: "documents");
        }
    }
}
