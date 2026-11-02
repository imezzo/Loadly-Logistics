using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class ChangeModelTypeToIntegerInDocumentsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {

        migrationBuilder.DropColumn(
            name: "model_type",
            table: "documents");


        migrationBuilder.AddColumn<int>(
            name: "entity_id",
            table: "documents",
            type: "integer",
            nullable: false,
            defaultValue: 1);


        migrationBuilder.CreateIndex(
            name: "IX_documents_entity_id",
            table: "documents",
            column: "entity_id");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {

        migrationBuilder.DropIndex(
            name: "IX_documents_entity_id",
            table: "documents");

        migrationBuilder.DropColumn(
            name: "entity_id",
            table: "documents");


        migrationBuilder.AddColumn<string>(
            name: "model_type",
            table: "documents",
            type: "character varying(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "");
    }
}
