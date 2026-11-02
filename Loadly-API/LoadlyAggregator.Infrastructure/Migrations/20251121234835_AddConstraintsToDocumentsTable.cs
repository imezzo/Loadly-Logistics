using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddConstraintsToDocumentsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "name_ar",
            table: "documents",
            type: "character varying(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "character varying(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_documents_name_ar",
            table: "documents",
            column: "name_ar",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_documents_name_en",
            table: "documents",
            column: "name_en",
            unique: true);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_documents_name_ar",
            table: "documents");

        migrationBuilder.DropIndex(
            name: "IX_documents_name_en",
            table: "documents");

        migrationBuilder.AlterColumn<string>(
            name: "name_ar",
            table: "documents",
            type: "character varying(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(255)",
            oldMaxLength: 255);
    }
}
