using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddPathToDriverDocuments : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "path",
            table: "driver_documents",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "path",
            table: "driver_documents");
    }
}
