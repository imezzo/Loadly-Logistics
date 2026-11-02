using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateDriverDetailsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "driver_details",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                driver_id = table.Column<long>(type: "bigint", nullable: false),
                date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                gender = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                license_type_id = table.Column<long>(type: "bigint", nullable: true),
                license_issue_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                license_no = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                license_expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                vehicle_type_ids = table.Column<string>(type: "text", nullable: true),
                age = table.Column<int>(type: "integer", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_driver_details", x => x.id);
                table.ForeignKey(
                    name: "FK_driver_details_drivers_driver_id",
                    column: x => x.driver_id,
                    principalTable: "drivers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_driver_details_driver_id",
            table: "driver_details",
            column: "driver_id");

        migrationBuilder.CreateIndex(
            name: "IX_driver_details_license_type_id",
            table: "driver_details",
            column: "license_type_id");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "driver_details");
    }
}
