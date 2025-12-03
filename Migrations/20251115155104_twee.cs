using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flauction.Migrations
{
    /// <inheritdoc />
    public partial class twee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Plant_supplier_id",
                table: "Plant",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_Media_plant_id",
                table: "Media",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPerson_company_id",
                table: "ContactPerson",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionLot_auction_id",
                table: "AuctionLot",
                column: "auction_id");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionLot_image_id",
                table: "AuctionLot",
                column: "image_id");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionClock_auction_id",
                table: "AuctionClock",
                column: "auction_id");

            migrationBuilder.CreateIndex(
                name: "IX_Auction_auctionmaster_id",
                table: "Auction",
                column: "auctionmaster_id");

            migrationBuilder.CreateIndex(
                name: "IX_Auction_plant_id",
                table: "Auction",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_Auction_winner_company_id",
                table: "Auction",
                column: "winner_company_id");

            migrationBuilder.CreateIndex(
                name: "IX_Acceptance_auction_id",
                table: "Acceptance",
                column: "auction_id");

            migrationBuilder.CreateIndex(
                name: "IX_Acceptance_auction_lot_id",
                table: "Acceptance",
                column: "auction_lot_id");

            migrationBuilder.CreateIndex(
                name: "IX_Acceptance_company_id",
                table: "Acceptance",
                column: "company_id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_Plant_supplier_id",
                table: "Plant");

            migrationBuilder.DropIndex(
                name: "IX_Media_plant_id",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_ContactPerson_company_id",
                table: "ContactPerson");

            migrationBuilder.DropIndex(
                name: "IX_AuctionLot_auction_id",
                table: "AuctionLot");

            migrationBuilder.DropIndex(
                name: "IX_AuctionLot_image_id",
                table: "AuctionLot");

            migrationBuilder.DropIndex(
                name: "IX_AuctionClock_auction_id",
                table: "AuctionClock");

            migrationBuilder.DropIndex(
                name: "IX_Auction_auctionmaster_id",
                table: "Auction");

            migrationBuilder.DropIndex(
                name: "IX_Auction_plant_id",
                table: "Auction");

            migrationBuilder.DropIndex(
                name: "IX_Auction_winner_company_id",
                table: "Auction");

            migrationBuilder.DropIndex(
                name: "IX_Acceptance_auction_id",
                table: "Acceptance");

            migrationBuilder.DropIndex(
                name: "IX_Acceptance_auction_lot_id",
                table: "Acceptance");

            migrationBuilder.DropIndex(
                name: "IX_Acceptance_company_id",
                table: "Acceptance");
        }
    }
}
