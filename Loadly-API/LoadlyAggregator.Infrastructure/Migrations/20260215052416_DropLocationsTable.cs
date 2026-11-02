using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class DropLocationsTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "locations");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "locations",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                city_id = table.Column<long>(type: "bigint", nullable: false),
                district_id = table.Column<long>(type: "bigint", nullable: true),
                governerate_id = table.Column<long>(type: "bigint", nullable: false),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                building_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                building_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                latitude = table.Column<decimal>(type: "numeric(10,8)", nullable: true),
                longitude = table.Column<decimal>(type: "numeric(11,8)", nullable: true),
                street_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_locations", x => x.id);
                table.ForeignKey(
                    name: "FK_locations_cities_city_id",
                    column: x => x.city_id,
                    principalTable: "cities",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_locations_districts_district_id",
                    column: x => x.district_id,
                    principalTable: "districts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_locations_governerates_governerate_id",
                    column: x => x.governerate_id,
                    principalTable: "governerates",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_locations_city_id",
            table: "locations",
            column: "city_id");

        migrationBuilder.CreateIndex(
            name: "IX_locations_district_id",
            table: "locations",
            column: "district_id");

        migrationBuilder.CreateIndex(
            name: "IX_locations_governerate_id",
            table: "locations",
            column: "governerate_id");

        migrationBuilder.CreateIndex(
            name: "IX_locations_latitude_longitude",
            table: "locations",
            columns: new[] { "latitude", "longitude" });
    }
}
