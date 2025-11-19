using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flauction.Migrations
{
    /// <inheritdoc />
    public partial class databasev2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "company_id1",
                table: "ContactPerson",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactPerson_company_id1",
                table: "ContactPerson",
                column: "company_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactPerson_Company_company_id1",
                table: "ContactPerson",
                column: "company_id1",
                principalTable: "Company",
                principalColumn: "company_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactPerson_Company_company_id1",
                table: "ContactPerson");

            migrationBuilder.DropIndex(
                name: "IX_ContactPerson_company_id1",
                table: "ContactPerson");

            migrationBuilder.DropColumn(
                name: "company_id1",
                table: "ContactPerson");
        }
    }
}
