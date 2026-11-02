using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTripAddedByToDispatcherAndAddCompanyUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "added_by",
                table: "trips",
                newName: "dispatcher_id");

            migrationBuilder.AddColumn<long>(
                name: "company_user_id",
                table: "trips",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trips_company_user_id",
                table: "trips",
                column: "company_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_trips_dispatcher_id",
                table: "trips",
                column: "dispatcher_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_admins_dispatcher_id",
                table: "trips",
                column: "dispatcher_id",
                principalTable: "admins",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_trips_companies_company_user_id",
                table: "trips",
                column: "company_user_id",
                principalTable: "companies",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_admins_dispatcher_id",
                table: "trips");

            migrationBuilder.DropForeignKey(
                name: "FK_trips_companies_company_user_id",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_company_user_id",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_dispatcher_id",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "company_user_id",
                table: "trips");

            migrationBuilder.RenameColumn(
                name: "dispatcher_id",
                table: "trips",
                newName: "added_by");
        }
    }
}
