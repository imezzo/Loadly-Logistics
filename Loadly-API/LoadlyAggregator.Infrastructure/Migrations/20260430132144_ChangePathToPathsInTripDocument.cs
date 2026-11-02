using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangePathToPathsInTripDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "trip_document_status",
                table: "trip_documents");

            migrationBuilder.RenameColumn(
                name: "rejection_reason",
                table: "trip_documents",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "path",
                table: "trip_documents",
                newName: "paths");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "trip_documents",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "paths",
                table: "trip_documents",
                newName: "path");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "trip_documents",
                newName: "rejection_reason");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "trip_documents",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "trip_document_status",
                table: "trip_documents",
                type: "integer",
                nullable: true);
        }
    }
}
