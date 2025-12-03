using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flauction.Migrations
{
    /// <inheritdoc />
    public partial class databasev9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionMaster_AspNetUsers_userId",
                table: "AuctionMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_Company_AspNetUsers_userId",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_Supplier_AspNetUsers_userId",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Supplier_userId",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Company_userId",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_AuctionMaster_userId",
                table: "AuctionMaster");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Supplier");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "AuctionMaster");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Supplier",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Company",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "AuctionMaster",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_userId",
                table: "Supplier",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_userId",
                table: "Company",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionMaster_userId",
                table: "AuctionMaster",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionMaster_AspNetUsers_userId",
                table: "AuctionMaster",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_AspNetUsers_userId",
                table: "Company",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Supplier_AspNetUsers_userId",
                table: "Supplier",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
