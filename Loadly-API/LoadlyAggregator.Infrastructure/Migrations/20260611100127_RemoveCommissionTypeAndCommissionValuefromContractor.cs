using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCommissionTypeAndCommissionValuefromContractor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_contractors_commission_type",
                table: "contractors");

            migrationBuilder.DropColumn(
                name: "commission_type",
                table: "contractors");

            migrationBuilder.DropColumn(
                name: "commission_value",
                table: "contractors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "commission_type",
                table: "contractors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "commission_value",
                table: "contractors",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_contractors_commission_type",
                table: "contractors",
                column: "commission_type");
        }
    }
}
