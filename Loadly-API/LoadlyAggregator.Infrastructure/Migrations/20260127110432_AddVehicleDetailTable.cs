using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddVehicleDetailTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_vehicles_documents_insurance_id",
            table: "vehicles");

        migrationBuilder.AddColumn<long>(
            name: "brand_id",
            table: "vehicles",
            type: "bigint",
            nullable: true);

        migrationBuilder.AddColumn<long>(
            name: "brand_model_id",
            table: "vehicles",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "vehicle_details",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                vehicle_id = table.Column<long>(type: "bigint", nullable: false),
                governorate_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                insurance_details = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                temperature_range_setting = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                cooling_unit_specification = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                temperature_monitoring_capability = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                maximum_load_height = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                internal_height = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                internal_width = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                internal_length = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                loading_mechanism_type = table.Column<int>(type: "integer", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_vehicle_details", x => x.id);
                table.ForeignKey(
                    name: "FK_vehicle_details_vehicles_vehicle_id",
                    column: x => x.vehicle_id,
                    principalTable: "vehicles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "vehicle_documents",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                document_id = table.Column<long>(type: "bigint", nullable: false),
                vehicle_id = table.Column<long>(type: "bigint", nullable: false),
                path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_vehicle_documents", x => x.id);
                table.ForeignKey(
                    name: "FK_vehicle_documents_documents_document_id",
                    column: x => x.document_id,
                    principalTable: "documents",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_vehicle_documents_vehicles_vehicle_id",
                    column: x => x.vehicle_id,
                    principalTable: "vehicles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_brand_id",
            table: "vehicles",
            column: "brand_id");

        migrationBuilder.CreateIndex(
            name: "IX_vehicles_brand_model_id",
            table: "vehicles",
            column: "brand_model_id");

        migrationBuilder.CreateIndex(
            name: "IX_vehicle_details_vehicle_id",
            table: "vehicle_details",
            column: "vehicle_id");

        migrationBuilder.CreateIndex(
            name: "IX_vehicle_documents_document_id",
            table: "vehicle_documents",
            column: "document_id");

        migrationBuilder.CreateIndex(
            name: "IX_vehicle_documents_status",
            table: "vehicle_documents",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "IX_vehicle_documents_vehicle_id",
            table: "vehicle_documents",
            column: "vehicle_id");

        migrationBuilder.AddForeignKey(
            name: "FK_vehicles_vehicle_brand_models_brand_model_id",
            table: "vehicles",
            column: "brand_model_id",
            principalTable: "vehicle_brand_models",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_vehicles_vehicle_brands_brand_id",
            table: "vehicles",
            column: "brand_id",
            principalTable: "vehicle_brands",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_vehicles_vehicles_insurance_insurance_id",
            table: "vehicles",
            column: "insurance_id",
            principalTable: "vehicles_insurance",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_vehicles_vehicle_brand_models_brand_model_id",
            table: "vehicles");

        migrationBuilder.DropForeignKey(
            name: "FK_vehicles_vehicle_brands_brand_id",
            table: "vehicles");

        migrationBuilder.DropForeignKey(
            name: "FK_vehicles_vehicles_insurance_insurance_id",
            table: "vehicles");

        migrationBuilder.DropTable(
            name: "vehicle_details");

        migrationBuilder.DropTable(
            name: "vehicle_documents");

        migrationBuilder.DropIndex(
            name: "IX_vehicles_brand_id",
            table: "vehicles");

        migrationBuilder.DropIndex(
            name: "IX_vehicles_brand_model_id",
            table: "vehicles");

        migrationBuilder.DropColumn(
            name: "brand_id",
            table: "vehicles");

        migrationBuilder.DropColumn(
            name: "brand_model_id",
            table: "vehicles");

        migrationBuilder.AddForeignKey(
            name: "FK_vehicles_documents_insurance_id",
            table: "vehicles",
            column: "insurance_id",
            principalTable: "documents",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);
    }
}
