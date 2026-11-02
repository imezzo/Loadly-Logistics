using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateCompanyDocumentsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "company_documents",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                document_id = table.Column<long>(type: "bigint", nullable: false),
                company_id = table.Column<long>(type: "bigint", nullable: false),
                expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_company_documents", x => x.id);
                table.ForeignKey(
                    name: "FK_company_documents_companies_company_id",
                    column: x => x.company_id,
                    principalTable: "companies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_company_documents_documents_document_id",
                    column: x => x.document_id,
                    principalTable: "documents",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_company_documents_company_id",
            table: "company_documents",
            column: "company_id");

        migrationBuilder.CreateIndex(
            name: "IX_company_documents_document_id",
            table: "company_documents",
            column: "document_id");

        migrationBuilder.CreateIndex(
            name: "IX_company_documents_expiry_date",
            table: "company_documents",
            column: "expiry_date");

        migrationBuilder.CreateIndex(
            name: "IX_company_documents_status",
            table: "company_documents",
            column: "status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "company_documents");
    }
}
