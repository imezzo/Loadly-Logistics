using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class RemoveModelIdFromDocumentsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {

        migrationBuilder.DropIndex(
            name: "IX_documents_model_id_model_type",
            table: "documents");


        migrationBuilder.DropColumn(
            name: "model_id",
            table: "documents");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {

        migrationBuilder.AddColumn<long>(
            name: "model_id",
            table: "documents",
            type: "bigint",
            nullable: false,
            defaultValue: 0L);


        migrationBuilder.CreateIndex(
            name: "IX_documents_model_id_model_type",
            table: "documents",
            columns: new[] { "model_id", "model_type" });
    }
}
