using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateContactsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "contacts",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                model_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                model_id = table.Column<long>(type: "bigint", nullable: false),
                primary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                position = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                communication_method_id = table.Column<long>(type: "bigint", nullable: true),
                added_by = table.Column<long>(type: "bigint", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_contacts", x => x.id);
                table.ForeignKey(
                    name: "FK_contacts_communication_methods_communication_method_id",
                    column: x => x.communication_method_id,
                    principalTable: "communication_methods",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_contacts_communication_method_id",
            table: "contacts",
            column: "communication_method_id");

        migrationBuilder.CreateIndex(
            name: "IX_contacts_model_id_model_type",
            table: "contacts",
            columns: new[] { "model_id", "model_type" });

        migrationBuilder.CreateIndex(
            name: "IX_contacts_primary",
            table: "contacts",
            column: "primary");
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "contacts");
    }
}
