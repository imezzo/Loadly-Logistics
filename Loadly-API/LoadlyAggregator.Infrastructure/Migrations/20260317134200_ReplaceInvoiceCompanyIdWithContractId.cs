using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceInvoiceCompanyIdWithContractId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoices_companies_company_id",
                table: "invoices");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "invoices",
                newName: "contract_id");

            migrationBuilder.RenameIndex(
                name: "IX_invoices_company_id_status",
                table: "invoices",
                newName: "IX_invoices_contract_id_status");

            migrationBuilder.RenameIndex(
                name: "IX_invoices_company_id",
                table: "invoices",
                newName: "IX_invoices_contract_id");

            migrationBuilder.AddForeignKey(
                name: "FK_invoices_contracts_contract_id",
                table: "invoices",
                column: "contract_id",
                principalTable: "contracts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoices_contracts_contract_id",
                table: "invoices");

            migrationBuilder.RenameColumn(
                name: "contract_id",
                table: "invoices",
                newName: "company_id");

            migrationBuilder.RenameIndex(
                name: "IX_invoices_contract_id_status",
                table: "invoices",
                newName: "IX_invoices_company_id_status");

            migrationBuilder.RenameIndex(
                name: "IX_invoices_contract_id",
                table: "invoices",
                newName: "IX_invoices_company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_invoices_companies_company_id",
                table: "invoices",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
