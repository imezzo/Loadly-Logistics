using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNationalIdAndSecondPhoneToContractos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "national_id",
                table: "contractors",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "second_phone_number",
                table: "contractors",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "contractor_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    document_id = table.Column<long>(type: "bigint", nullable: false),
                    contractor_id = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_contractor_documents", x => x.id);
                    table.ForeignKey(
                        name: "FK_contractor_documents_contractors_contractor_id",
                        column: x => x.contractor_id,
                        principalTable: "contractors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contractor_documents_documents_document_id",
                        column: x => x.document_id,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contractors_national_id",
                table: "contractors",
                column: "national_id",
                unique: true,
                filter: "\"national_id\" IS NOT NULL AND \"national_id\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_contractor_documents_contractor_id",
                table: "contractor_documents",
                column: "contractor_id");

            migrationBuilder.CreateIndex(
                name: "IX_contractor_documents_document_id",
                table: "contractor_documents",
                column: "document_id");

            migrationBuilder.CreateIndex(
                name: "IX_contractor_documents_status",
                table: "contractor_documents",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contractor_documents");

            migrationBuilder.DropIndex(
                name: "IX_contractors_national_id",
                table: "contractors");

            migrationBuilder.DropColumn(
                name: "national_id",
                table: "contractors");

            migrationBuilder.DropColumn(
                name: "second_phone_number",
                table: "contractors");
        }
    }
}
