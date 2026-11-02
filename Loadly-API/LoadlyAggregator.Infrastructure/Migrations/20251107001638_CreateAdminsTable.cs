using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations;


public partial class CreateAdminsTable : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "admins",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                type = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                mfa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                email_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                phone_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                status = table.Column<int>(type: "integer", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_admins", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "companies",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                type = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                mfa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                email_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                phone_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                status = table.Column<int>(type: "integer", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_companies", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "drivers",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                mfa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                email_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                phone_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                status = table.Column<int>(type: "integer", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_drivers", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "permission_groups",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                status = table.Column<int>(type: "integer", nullable: false),
                type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "company"),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_permission_groups", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "roles",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                guard_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_roles", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "permissions",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                guard_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                permission_group_id = table.Column<long>(type: "bigint", nullable: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_permissions", x => x.id);
                table.ForeignKey(
                    name: "FK_permissions_permission_groups_permission_group_id",
                    column: x => x.permission_group_id,
                    principalTable: "permission_groups",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "model_has_roles",
            columns: table => new
            {
                role_id = table.Column<long>(type: "bigint", nullable: false),
                model_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                model_id = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_model_has_roles", x => new { x.role_id, x.model_id, x.model_type });
                table.ForeignKey(
                    name: "FK_model_has_roles_roles_role_id",
                    column: x => x.role_id,
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "model_has_permissions",
            columns: table => new
            {
                permission_id = table.Column<long>(type: "bigint", nullable: false),
                model_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                model_id = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_model_has_permissions", x => new { x.permission_id, x.model_id, x.model_type });
                table.ForeignKey(
                    name: "FK_model_has_permissions_permissions_permission_id",
                    column: x => x.permission_id,
                    principalTable: "permissions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "role_has_permissions",
            columns: table => new
            {
                permission_id = table.Column<long>(type: "bigint", nullable: false),
                role_id = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_role_has_permissions", x => new { x.permission_id, x.role_id });
                table.ForeignKey(
                    name: "FK_role_has_permissions_permissions_permission_id",
                    column: x => x.permission_id,
                    principalTable: "permissions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_role_has_permissions_roles_role_id",
                    column: x => x.role_id,
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_admins_email",
            table: "admins",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_admins_phone",
            table: "admins",
            column: "phone",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_admins_status",
            table: "admins",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "IX_admins_type",
            table: "admins",
            column: "type");

        migrationBuilder.CreateIndex(
            name: "IX_admins_username",
            table: "admins",
            column: "username",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_companies_email",
            table: "companies",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_companies_phone",
            table: "companies",
            column: "phone",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_companies_status",
            table: "companies",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "IX_companies_type",
            table: "companies",
            column: "type");

        migrationBuilder.CreateIndex(
            name: "IX_companies_username",
            table: "companies",
            column: "username",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_drivers_email",
            table: "drivers",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_drivers_phone",
            table: "drivers",
            column: "phone",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_drivers_status",
            table: "drivers",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "IX_drivers_username",
            table: "drivers",
            column: "username",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_model_has_permissions_model_id_model_type",
            table: "model_has_permissions",
            columns: new[] { "model_id", "model_type" });

        migrationBuilder.CreateIndex(
            name: "IX_model_has_roles_model_id_model_type",
            table: "model_has_roles",
            columns: new[] { "model_id", "model_type" });

        migrationBuilder.CreateIndex(
            name: "IX_permission_groups_type",
            table: "permission_groups",
            column: "type");

        migrationBuilder.CreateIndex(
            name: "IX_permissions_name_guard_name",
            table: "permissions",
            columns: new[] { "name", "guard_name" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_permissions_permission_group_id",
            table: "permissions",
            column: "permission_group_id");

        migrationBuilder.CreateIndex(
            name: "IX_role_has_permissions_role_id",
            table: "role_has_permissions",
            column: "role_id");

        migrationBuilder.CreateIndex(
            name: "IX_roles_guard_name",
            table: "roles",
            column: "guard_name");

        migrationBuilder.CreateIndex(
            name: "IX_roles_name",
            table: "roles",
            column: "name");

        migrationBuilder.CreateIndex(
            name: "IX_roles_name_guard_name",
            table: "roles",
            columns: new[] { "name", "guard_name" },
            unique: true);
    }


    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "model_has_permissions");

        migrationBuilder.DropTable(
            name: "model_has_roles");

        migrationBuilder.DropTable(
            name: "role_has_permissions");

        migrationBuilder.DropTable(
            name: "admins");

        migrationBuilder.DropTable(
            name: "companies");

        migrationBuilder.DropTable(
            name: "drivers");

        migrationBuilder.DropTable(
            name: "permissions");

        migrationBuilder.DropTable(
            name: "roles");

        migrationBuilder.DropTable(
            name: "permission_groups");
    }
}
