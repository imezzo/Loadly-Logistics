using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class AddContractApprovalWorkflow : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "current_approval_stage",
            table: "contracts",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "deactivation_date",
            table: "contracts",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "deactivation_notes",
            table: "contracts",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "deactivation_reason",
            table: "contracts",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "rejection_reason",
            table: "contracts",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<long>(
            name: "renewed_from_contract_id",
            table: "contracts",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "contract_approvals",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                contract_id = table.Column<long>(type: "bigint", nullable: false),
                stage = table.Column<int>(type: "integer", nullable: false),
                approver_id = table.Column<long>(type: "bigint", nullable: true),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                comments = table.Column<string>(type: "text", nullable: true),
                action_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                stage_order = table.Column<int>(type: "integer", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_contract_approvals", x => x.id);
                table.ForeignKey(
                    name: "FK_contract_approvals_admins_approver_id",
                    column: x => x.approver_id,
                    principalTable: "admins",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_contract_approvals_contracts_contract_id",
                    column: x => x.contract_id,
                    principalTable: "contracts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "contract_status_histories",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                contract_id = table.Column<long>(type: "bigint", nullable: false),
                previous_status = table.Column<int>(type: "integer", nullable: false),
                new_status = table.Column<int>(type: "integer", nullable: false),
                reason = table.Column<string>(type: "text", nullable: true),
                changed_by = table.Column<long>(type: "bigint", nullable: true),
                changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                additional_notes = table.Column<string>(type: "text", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_contract_status_histories", x => x.id);
                table.ForeignKey(
                    name: "FK_contract_status_histories_admins_changed_by",
                    column: x => x.changed_by,
                    principalTable: "admins",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_contract_status_histories_contracts_contract_id",
                    column: x => x.contract_id,
                    principalTable: "contracts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_contracts_renewed_from_contract_id",
            table: "contracts",
            column: "renewed_from_contract_id");

        migrationBuilder.CreateIndex(
            name: "IX_contract_approvals_approver_id",
            table: "contract_approvals",
            column: "approver_id");

        migrationBuilder.CreateIndex(
            name: "IX_contract_approvals_contract_id",
            table: "contract_approvals",
            column: "contract_id");

        migrationBuilder.CreateIndex(
            name: "IX_contract_approvals_contract_id_stage",
            table: "contract_approvals",
            columns: new[] { "contract_id", "stage" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_contract_approvals_status",
            table: "contract_approvals",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "IX_contract_status_histories_changed_at",
            table: "contract_status_histories",
            column: "changed_at");

        migrationBuilder.CreateIndex(
            name: "IX_contract_status_histories_changed_by",
            table: "contract_status_histories",
            column: "changed_by");

        migrationBuilder.CreateIndex(
            name: "IX_contract_status_histories_contract_id",
            table: "contract_status_histories",
            column: "contract_id");

        migrationBuilder.AddForeignKey(
            name: "FK_contracts_contracts_renewed_from_contract_id",
            table: "contracts",
            column: "renewed_from_contract_id",
            principalTable: "contracts",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_contracts_contracts_renewed_from_contract_id",
            table: "contracts");

        migrationBuilder.DropTable(
            name: "contract_approvals");

        migrationBuilder.DropTable(
            name: "contract_status_histories");

        migrationBuilder.DropIndex(
            name: "IX_contracts_renewed_from_contract_id",
            table: "contracts");

        migrationBuilder.DropColumn(
            name: "current_approval_stage",
            table: "contracts");

        migrationBuilder.DropColumn(
            name: "deactivation_date",
            table: "contracts");

        migrationBuilder.DropColumn(
            name: "deactivation_notes",
            table: "contracts");

        migrationBuilder.DropColumn(
            name: "deactivation_reason",
            table: "contracts");

        migrationBuilder.DropColumn(
            name: "rejection_reason",
            table: "contracts");

        migrationBuilder.DropColumn(
            name: "renewed_from_contract_id",
            table: "contracts");
    }
}
