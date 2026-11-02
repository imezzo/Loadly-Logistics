using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateAddressesTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "addresses",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                model_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                model_id = table.Column<long>(type: "bigint", nullable: false),
                street_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                building_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                building_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                governerate_id = table.Column<long>(type: "bigint", nullable: true),
                city_id = table.Column<long>(type: "bigint", nullable: true),
                district_id = table.Column<long>(type: "bigint", nullable: true),
                postal_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_addresses", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_addresses_model_id_model_type",
            table: "addresses",
            columns: new[] { "model_id", "model_type" });

        migrationBuilder.CreateIndex(
            name: "IX_addresses_status",
            table: "addresses",
            column: "status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "addresses");
    }
}
