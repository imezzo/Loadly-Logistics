using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTargetsUniqueIndexIncludeTargetType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_targets_month_year_unique",
                table: "targets");

            migrationBuilder.CreateIndex(
                name: "ix_targets_month_year_target_type_unique",
                table: "targets",
                columns: new[] { "month", "year", "target_type" },
                unique: true,
                filter: "\"deleted_at\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_targets_month_year_target_type_unique",
                table: "targets");

            migrationBuilder.CreateIndex(
                name: "ix_targets_month_year_unique",
                table: "targets",
                columns: new[] { "month", "year" },
                unique: true,
                filter: "\"deleted_at\" IS NULL");
        }
    }
}
