using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCollectorsTableAndRetargetTripFks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_admins_collector_id",
                table: "trips");

            migrationBuilder.DropForeignKey(
                name: "FK_trips_admins_finance_collector_id",
                table: "trips");

            migrationBuilder.CreateTable(
                name: "collectors",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    mfa_channel = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    email_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    phone_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collectors", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_collectors_email",
                table: "collectors",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_collectors_phone",
                table: "collectors",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_collectors_status",
                table: "collectors",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_collectors_username",
                table: "collectors",
                column: "username",
                unique: true);

            // Polymorphic tables (addresses, contacts, roles) use model_type + model_id;
            // do not add DB FKs on model_id alone (same id can exist across user types).

            // migrationBuilder.AddForeignKey(
            //     name: "FK_trips_collectors_collector_id",
            //     table: "trips",
            //     column: "collector_id",
            //     principalTable: "collectors",
            //     principalColumn: "id",
            //     onDelete: ReferentialAction.SetNull);

            // migrationBuilder.AddForeignKey(
            //     name: "FK_trips_collectors_finance_collector_id",
            //     table: "trips",
            //     column: "finance_collector_id",
            //     principalTable: "collectors",
            //     principalColumn: "id",
            //     onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_collectors_collector_id",
                table: "trips");

            migrationBuilder.DropForeignKey(
                name: "FK_trips_collectors_finance_collector_id",
                table: "trips");

            migrationBuilder.DropTable(
                name: "collectors");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_trips_admins_collector_id",
            //     table: "trips",
            //     column: "collector_id",
            //     principalTable: "admins",
            //     principalColumn: "id",
            //     onDelete: ReferentialAction.SetNull);

            // migrationBuilder.AddForeignKey(
            //     name: "FK_trips_admins_finance_collector_id",
            //     table: "trips",
            //     column: "finance_collector_id",
            //     principalTable: "admins",
            //     principalColumn: "id",
            //     onDelete: ReferentialAction.SetNull);
        }
    }
}
