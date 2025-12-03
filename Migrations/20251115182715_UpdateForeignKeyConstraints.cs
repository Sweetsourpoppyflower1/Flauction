using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flauction.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acceptance_AuctionLot_auction_lot_id",
                table: "Acceptance");

            migrationBuilder.DropForeignKey(
                name: "FK_Acceptance_Auction_auction_id",
                table: "Acceptance");

            migrationBuilder.DropForeignKey(
                name: "FK_Acceptance_Company_company_id",
                table: "Acceptance");

            migrationBuilder.DropForeignKey(
                name: "FK_Auction_AuctionMaster_auctionmaster_id",
                table: "Auction");

            migrationBuilder.DropForeignKey(
                name: "FK_Auction_Company_winner_company_id",
                table: "Auction");

            migrationBuilder.DropForeignKey(
                name: "FK_Auction_Plant_plant_id",
                table: "Auction");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionClock_Auction_auction_id",
                table: "AuctionClock");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionLot_Auction_auction_id",
                table: "AuctionLot");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionLot_Media_image_id",
                table: "AuctionLot");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactPerson_Company_company_id",
                table: "ContactPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_Plant_plant_id",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Plant_Supplier_supplier_id",
                table: "Plant");

            migrationBuilder.AddForeignKey(
                name: "FK_Acceptance_Auction",
                table: "Acceptance",
                column: "auction_id",
                principalTable: "Auction",
                principalColumn: "auction_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Acceptance_AuctionLot",
                table: "Acceptance",
                column: "auction_lot_id",
                principalTable: "AuctionLot",
                principalColumn: "auctionlot_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Acceptance_Company",
                table: "Acceptance",
                column: "company_id",
                principalTable: "Company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_AuctionMaster",
                table: "Auction",
                column: "auctionmaster_id",
                principalTable: "AuctionMaster",
                principalColumn: "auctionmaster_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_Plant",
                table: "Auction",
                column: "plant_id",
                principalTable: "Plant",
                principalColumn: "plant_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_WinnerCompany",
                table: "Auction",
                column: "winner_company_id",
                principalTable: "Company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionClock_Auction",
                table: "AuctionClock",
                column: "auction_id",
                principalTable: "Auction",
                principalColumn: "auction_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionLot_Auction",
                table: "AuctionLot",
                column: "auction_id",
                principalTable: "Auction",
                principalColumn: "auction_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionLot_Media",
                table: "AuctionLot",
                column: "image_id",
                principalTable: "Media",
                principalColumn: "media_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactPerson_Company",
                table: "ContactPerson",
                column: "company_id",
                principalTable: "Company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Plant",
                table: "Media",
                column: "plant_id",
                principalTable: "Plant",
                principalColumn: "plant_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Plant_Supplier",
                table: "Plant",
                column: "supplier_id",
                principalTable: "Supplier",
                principalColumn: "supplier_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acceptance_Auction",
                table: "Acceptance");

            migrationBuilder.DropForeignKey(
                name: "FK_Acceptance_AuctionLot",
                table: "Acceptance");

            migrationBuilder.DropForeignKey(
                name: "FK_Acceptance_Company",
                table: "Acceptance");

            migrationBuilder.DropForeignKey(
                name: "FK_Auction_AuctionMaster",
                table: "Auction");

            migrationBuilder.DropForeignKey(
                name: "FK_Auction_Plant",
                table: "Auction");

            migrationBuilder.DropForeignKey(
                name: "FK_Auction_WinnerCompany",
                table: "Auction");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionClock_Auction",
                table: "AuctionClock");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionLot_Auction",
                table: "AuctionLot");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionLot_Media",
                table: "AuctionLot");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactPerson_Company",
                table: "ContactPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_Plant",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Plant_Supplier",
                table: "Plant");

            migrationBuilder.AddForeignKey(
                name: "FK_Acceptance_AuctionLot_auction_lot_id",
                table: "Acceptance",
                column: "auction_lot_id",
                principalTable: "AuctionLot",
                principalColumn: "auctionlot_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Acceptance_Auction_auction_id",
                table: "Acceptance",
                column: "auction_id",
                principalTable: "Auction",
                principalColumn: "auction_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Acceptance_Company_company_id",
                table: "Acceptance",
                column: "company_id",
                principalTable: "Company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_AuctionMaster_auctionmaster_id",
                table: "Auction",
                column: "auctionmaster_id",
                principalTable: "AuctionMaster",
                principalColumn: "auctionmaster_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_Company_winner_company_id",
                table: "Auction",
                column: "winner_company_id",
                principalTable: "Company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_Plant_plant_id",
                table: "Auction",
                column: "plant_id",
                principalTable: "Plant",
                principalColumn: "plant_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionClock_Auction_auction_id",
                table: "AuctionClock",
                column: "auction_id",
                principalTable: "Auction",
                principalColumn: "auction_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionLot_Auction_auction_id",
                table: "AuctionLot",
                column: "auction_id",
                principalTable: "Auction",
                principalColumn: "auction_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionLot_Media_image_id",
                table: "AuctionLot",
                column: "image_id",
                principalTable: "Media",
                principalColumn: "media_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactPerson_Company_company_id",
                table: "ContactPerson",
                column: "company_id",
                principalTable: "Company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Plant_plant_id",
                table: "Media",
                column: "plant_id",
                principalTable: "Plant",
                principalColumn: "plant_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Plant_Supplier_supplier_id",
                table: "Plant",
                column: "supplier_id",
                principalTable: "Supplier",
                principalColumn: "supplier_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
