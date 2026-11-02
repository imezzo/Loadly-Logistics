using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddUniqueConstraintsToGoverneratesTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_governerates_name_ar",
            table: "governerates",
            column: "name_ar",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_governerates_name_en",
            table: "governerates",
            column: "name_en",
            unique: true);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_governerates_name_ar",
            table: "governerates");

        migrationBuilder.DropIndex(
            name: "IX_governerates_name_en",
            table: "governerates");
    }
}
