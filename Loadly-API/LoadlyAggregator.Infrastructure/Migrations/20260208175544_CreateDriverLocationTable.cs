using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class CreateDriverLocationTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "driver_locations",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                driver_id = table.Column<long>(type: "bigint", nullable: false),
                latitude = table.Column<decimal>(type: "numeric(10,8)", nullable: false),
                longitude = table.Column<decimal>(type: "numeric(11,8)", nullable: false),
                accuracy = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                heading = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                speed = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                recorded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_driver_locations", x => x.id);
                table.ForeignKey(
                    name: "FK_driver_locations_drivers_driver_id",
                    column: x => x.driver_id,
                    principalTable: "drivers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_driver_locations_driver_id",
            table: "driver_locations",
            column: "driver_id");

        migrationBuilder.CreateIndex(
            name: "IX_driver_locations_recorded_at",
            table: "driver_locations",
            column: "recorded_at");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "driver_locations");
    }
}
