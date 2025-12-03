using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flauction.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Acceptance",
                columns: table => new
                {
                    acceptance_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    auction_id = table.Column<int>(type: "int", nullable: false),
                    company_id = table.Column<int>(type: "int", nullable: false),
                    auction_lot_id = table.Column<int>(type: "int", nullable: false),
                    acc_tick_number = table.Column<int>(type: "int", nullable: false),
                    acc_accepted_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    acc_accepted_quantity = table.Column<int>(type: "int", nullable: false),
                    acc_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acceptance", x => x.acceptance_id);
                });

            migrationBuilder.CreateTable(
                name: "Auction",
                columns: table => new
                {
                    auction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    auctionmaster_id = table.Column<int>(type: "int", nullable: false),
                    plant_id = table.Column<int>(type: "int", nullable: false),
                    winner_company_id = table.Column<int>(type: "int", nullable: false),
                    au_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    au_start_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    au_end_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    au_start_price = table.Column<int>(type: "int", nullable: false),
                    au_current_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    au_min_increment = table.Column<int>(type: "int", nullable: false),
                    au_final_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auction", x => x.auction_id);
                });

            migrationBuilder.CreateTable(
                name: "AuctionClock",
                columns: table => new
                {
                    auctionclock_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    auction_id = table.Column<int>(type: "int", nullable: false),
                    ac_tick_interval_seconds = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ac_decrement_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ac_final_call_seconds = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionClock", x => x.auctionclock_id);
                });

            migrationBuilder.CreateTable(
                name: "AuctionLot",
                columns: table => new
                {
                    auctionlot_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    auction_id = table.Column<int>(type: "int", nullable: false),
                    image_id = table.Column<int>(type: "int", nullable: false),
                    al_unit_per_container = table.Column<int>(type: "int", nullable: false),
                    al_containers_in_lot = table.Column<int>(type: "int", nullable: false),
                    al_min_pickup = table.Column<int>(type: "int", nullable: false),
                    al_fustcode = table.Column<int>(type: "int", nullable: false),
                    al_total_quantity = table.Column<int>(type: "int", nullable: false),
                    al_remaining_quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionLot", x => x.auctionlot_id);
                });

            migrationBuilder.CreateTable(
                name: "AuctionMaster",
                columns: table => new
                {
                    auctionmaster_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    am_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    am_phone = table.Column<int>(type: "int", nullable: false),
                    am_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    am_address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    am_password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionMaster", x => x.auctionmaster_id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    company_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    c_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    c_address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    c_postalcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    c_country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    c_vat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    c_iban = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    c_bicswift = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    c_password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.company_id);
                });

            migrationBuilder.CreateTable(
                name: "ContactPerson",
                columns: table => new
                {
                    contactperson_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    company_id = table.Column<int>(type: "int", nullable: false),
                    cp_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cp_phone = table.Column<int>(type: "int", nullable: false),
                    cp_email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPerson", x => x.contactperson_id);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    media_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    plant_id = table.Column<int>(type: "int", nullable: false),
                    m_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    m_alt_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    m_is_primary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.media_id);
                });

            migrationBuilder.CreateTable(
                name: "Plant",
                columns: table => new
                {
                    plant_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    supplier_id = table.Column<int>(type: "int", nullable: false),
                    p_productname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    p_category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    p_form = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    p_quality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    p_min_stem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    p_stems_bunch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    p_maturity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    p_desc = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plant", x => x.plant_id);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    supplier_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    s_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    s_address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    s_postalcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    s_country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    s_iban = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    s_desc = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.supplier_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Acceptance");

            migrationBuilder.DropTable(
                name: "Auction");

            migrationBuilder.DropTable(
                name: "AuctionClock");

            migrationBuilder.DropTable(
                name: "AuctionLot");

            migrationBuilder.DropTable(
                name: "AuctionMaster");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "ContactPerson");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "Plant");

            migrationBuilder.DropTable(
                name: "Supplier");
        }
    }
}
