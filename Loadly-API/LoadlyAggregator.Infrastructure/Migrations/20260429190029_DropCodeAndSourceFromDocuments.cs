using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropCodeAndSourceFromDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_documents_entity_id_code",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "IX_documents_entity_id_source",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "code",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "source",
                table: "documents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "documents",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "source",
                table: "documents",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_documents_entity_id_code",
                table: "documents",
                columns: new[] { "entity_id", "code" },
                unique: true,
                filter: "\"code\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_documents_entity_id_source",
                table: "documents",
                columns: new[] { "entity_id", "source" });
        }
    }
}
