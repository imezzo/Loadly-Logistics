using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateContractDocumentsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "contract_documents",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                contract_id = table.Column<long>(type: "bigint", nullable: false),
                document_id = table.Column<long>(type: "bigint", nullable: false),
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
                table.PrimaryKey("PK_contract_documents", x => x.id);
                table.ForeignKey(
                    name: "FK_contract_documents_contracts_contract_id",
                    column: x => x.contract_id,
                    principalTable: "contracts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_contract_documents_documents_document_id",
                    column: x => x.document_id,
                    principalTable: "documents",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_contract_documents_contract_id",
            table: "contract_documents",
            column: "contract_id");

        migrationBuilder.CreateIndex(
            name: "IX_contract_documents_document_id",
            table: "contract_documents",
            column: "document_id");

        migrationBuilder.CreateIndex(
            name: "IX_contract_documents_expiry_date",
            table: "contract_documents",
            column: "expiry_date");

        migrationBuilder.CreateIndex(
            name: "IX_contract_documents_status",
            table: "contract_documents",
            column: "status");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "contract_documents");
    }
}
