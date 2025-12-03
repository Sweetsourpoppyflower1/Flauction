using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flauction.Migrations
{
    /// <inheritdoc />
    public partial class databasev3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auction_WinnerCompany",
                table: "Auction");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactPerson_Company_company_id1",
                table: "ContactPerson");

            migrationBuilder.DropIndex(
                name: "IX_ContactPerson_company_id1",
                table: "ContactPerson");

            migrationBuilder.DropIndex(
                name: "IX_Auction_winner_company_id",
                table: "Auction");

            migrationBuilder.DropColumn(
                name: "company_id1",
                table: "ContactPerson");

            migrationBuilder.DropColumn(
                name: "au_current_price",
                table: "Auction");

            migrationBuilder.DropColumn(
                name: "au_min_increment",
                table: "Auction");

            migrationBuilder.DropColumn(
                name: "winner_company_id",
                table: "Auction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "company_id1",
                table: "ContactPerson",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "au_current_price",
                table: "Auction",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "au_min_increment",
                table: "Auction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "winner_company_id",
                table: "Auction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ContactPerson_company_id1",
                table: "ContactPerson",
                column: "company_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Auction_winner_company_id",
                table: "Auction",
                column: "winner_company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_WinnerCompany",
                table: "Auction",
                column: "winner_company_id",
                principalTable: "Company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactPerson_Company_company_id1",
                table: "ContactPerson",
                column: "company_id1",
                principalTable: "Company",
                principalColumn: "company_id");
        }
    }
}
