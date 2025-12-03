using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flauction.Migrations
{
    /// <inheritdoc />
    public partial class databasev12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acceptance_Company",
                table: "Acceptance");

            migrationBuilder.DropForeignKey(
                name: "FK_Auction_AuctionMaster",
                table: "Auction");

            migrationBuilder.DropForeignKey(
                name: "FK_Plant_Supplier",
                table: "Plant");

            migrationBuilder.DropTable(
                name: "AuctionClock");

            migrationBuilder.DropTable(
                name: "ContactPerson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Supplier",
                table: "Supplier");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Company",
                table: "Company");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuctionMaster",
                table: "AuctionMaster");

            migrationBuilder.DropColumn(
                name: "supplier_id",
                table: "Supplier");

            migrationBuilder.DropColumn(
                name: "s_email",
                table: "Supplier");

            migrationBuilder.DropColumn(
                name: "s_name",
                table: "Supplier");

            migrationBuilder.DropColumn(
                name: "s_password",
                table: "Supplier");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "c_name",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "c_password",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "auctionmaster_id",
                table: "AuctionMaster");

            migrationBuilder.DropColumn(
                name: "am_email",
                table: "AuctionMaster");

            migrationBuilder.DropColumn(
                name: "am_name",
                table: "AuctionMaster");

            migrationBuilder.DropColumn(
                name: "am_password",
                table: "AuctionMaster");

            migrationBuilder.DropColumn(
                name: "am_phone",
                table: "AuctionMaster");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Supplier",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "supplier_id",
                table: "Plant",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Company",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AuctionMaster",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "auctionmaster_id",
                table: "Auction",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "company_id",
                table: "Acceptance",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Supplier",
                table: "Supplier",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company",
                table: "Company",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuctionMaster",
                table: "AuctionMaster",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Acceptance_Company",
                table: "Acceptance",
                column: "company_id",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_AuctionMaster",
                table: "Auction",
                column: "auctionmaster_id",
                principalTable: "AuctionMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Plant_Supplier",
                table: "Plant",
                column: "supplier_id",
                principalTable: "Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acceptance_Company",
                table: "Acceptance");

            migrationBuilder.DropForeignKey(
                name: "FK_Auction_AuctionMaster",
                table: "Auction");

            migrationBuilder.DropForeignKey(
                name: "FK_Plant_Supplier",
                table: "Plant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Supplier",
                table: "Supplier");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Company",
                table: "Company");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuctionMaster",
                table: "AuctionMaster");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "supplier_id",
                table: "Supplier",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "s_email",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "s_name",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "s_password",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "supplier_id",
                table: "Plant",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "company_id",
                table: "Company",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "c_name",
                table: "Company",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "c_password",
                table: "Company",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AuctionMaster",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "auctionmaster_id",
                table: "AuctionMaster",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "am_email",
                table: "AuctionMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "am_name",
                table: "AuctionMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "am_password",
                table: "AuctionMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "am_phone",
                table: "AuctionMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "auctionmaster_id",
                table: "Auction",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "company_id",
                table: "Acceptance",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Supplier",
                table: "Supplier",
                column: "supplier_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company",
                table: "Company",
                column: "company_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuctionMaster",
                table: "AuctionMaster",
                column: "auctionmaster_id");

            migrationBuilder.CreateTable(
                name: "AuctionClock",
                columns: table => new
                {
                    auctionclock_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ac_decrement_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ac_final_call_seconds = table.Column<int>(type: "int", nullable: false),
                    ac_tick_interval_seconds = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    auction_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionClock", x => x.auctionclock_id);
                    table.ForeignKey(
                        name: "FK_AuctionClock_Auction",
                        column: x => x.auction_id,
                        principalTable: "Auction",
                        principalColumn: "auction_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContactPerson",
                columns: table => new
                {
                    contactperson_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    company_id = table.Column<int>(type: "int", nullable: false),
                    cp_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cp_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cp_phone = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPerson", x => x.contactperson_id);
                    table.ForeignKey(
                        name: "FK_ContactPerson_Company",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuctionClock_auction_id",
                table: "AuctionClock",
                column: "auction_id");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPerson_company_id",
                table: "ContactPerson",
                column: "company_id");

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
                name: "FK_Plant_Supplier",
                table: "Plant",
                column: "supplier_id",
                principalTable: "Supplier",
                principalColumn: "supplier_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
