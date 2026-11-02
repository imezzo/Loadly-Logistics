using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceContractTypesTableWithEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contracts_contract_types_contract_type_id",
                table: "contracts");

            migrationBuilder.DropTable(
                name: "contract_types");

            migrationBuilder.AlterColumn<int>(
                name: "contract_type_id",
                table: "contracts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "contract_type_id",
                table: "contracts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "contract_types",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    added_by = table.Column<long>(type: "bigint", nullable: true),
                    best_for_ar = table.Column<string>(type: "text", nullable: true),
                    best_for_en = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    desc_ar = table.Column<string>(type: "text", nullable: true),
                    desc_en = table.Column<string>(type: "text", nullable: true),
                    name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract_types", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contract_types_status",
                table: "contract_types",
                column: "status");

            migrationBuilder.AddForeignKey(
                name: "FK_contracts_contract_types_contract_type_id",
                table: "contracts",
                column: "contract_type_id",
                principalTable: "contract_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
