using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flauction.Migrations
{
    /// <inheritdoc />
    public partial class RenameAuctionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Acceptance renames
            migrationBuilder.RenameColumn(name: "acc_time", table: "Acceptance", newName: "time");
            migrationBuilder.RenameColumn(name: "acc_tick_number", table: "Acceptance", newName: "tick_number");
            migrationBuilder.RenameColumn(name: "acc_accepted_price", table: "Acceptance", newName: "accepted_price");
            migrationBuilder.RenameColumn(name: "acc_accepted_quantity", table: "Acceptance", newName: "accepted_quantity");

            // Auction renames and type change for FK to Identity Id (string)
            migrationBuilder.RenameColumn(name: "au_status", table: "Auction", newName: "status");
            migrationBuilder.RenameColumn(name: "au_start_time", table: "Auction", newName: "start_time");
            migrationBuilder.RenameColumn(name: "au_end_time", table: "Auction", newName: "end_time");
            migrationBuilder.RenameColumn(name: "au_start_price", table: "Auction", newName: "start_price");
            migrationBuilder.RenameColumn(name: "au_final_price", table: "Auction", newName: "final_price");
            migrationBuilder.RenameColumn(name: "au_min_increment", table: "Auction", newName: "min_price");

            migrationBuilder.AlterColumn<string>(
                name: "auctionmaster_id",
                table: "Auction",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // AuctionLot renames
            migrationBuilder.RenameColumn(name: "al_unit_per_container", table: "AuctionLot", newName: "unit_per_container");
            migrationBuilder.RenameColumn(name: "al_containers_in_lot", table: "AuctionLot", newName: "containers_in_lot");
            migrationBuilder.RenameColumn(name: "al_min_pickup", table: "AuctionLot", newName: "min_pickup");
            migrationBuilder.RenameColumn(name: "al_fustcode", table: "AuctionLot", newName: "fustcode");
            migrationBuilder.RenameColumn(name: "al_total_quantity", table: "AuctionLot", newName: "total_quantity");
            migrationBuilder.RenameColumn(name: "al_remaining_quantity", table: "AuctionLot", newName: "remaining_quantity");

            // Media renames
            migrationBuilder.RenameColumn(name: "m_url", table: "Media", newName: "url");
            migrationBuilder.RenameColumn(name: "m_alt_text", table: "Media", newName: "alt_text");
            migrationBuilder.RenameColumn(name: "m_is_primary", table: "Media", newName: "is_primary");

            // Plant renames
            migrationBuilder.RenameColumn(name: "p_productname", table: "Plant", newName: "productname");
            migrationBuilder.RenameColumn(name: "p_category", table: "Plant", newName: "category");
            migrationBuilder.RenameColumn(name: "p_form", table: "Plant", newName: "form");
            migrationBuilder.RenameColumn(name: "p_quality", table: "Plant", newName: "quality");
            migrationBuilder.RenameColumn(name: "p_min_stem", table: "Plant", newName: "min_stem");
            migrationBuilder.RenameColumn(name: "p_stems_bunch", table: "Plant", newName: "stems_bunch");
            migrationBuilder.RenameColumn(name: "p_maturity", table: "Plant", newName: "maturity");
            migrationBuilder.RenameColumn(name: "p_desc", table: "Plant", newName: "desc");

            // Company renames
            migrationBuilder.RenameColumn(name: "c_address", table: "Company", newName: "address");
            migrationBuilder.RenameColumn(name: "c_postalcode", table: "Company", newName: "postalcode");
            migrationBuilder.RenameColumn(name: "c_country", table: "Company", newName: "country");
            migrationBuilder.RenameColumn(name: "c_vat", table: "Company", newName: "vat");
            migrationBuilder.RenameColumn(name: "c_iban", table: "Company", newName: "iban");
            migrationBuilder.RenameColumn(name: "c_bicswift", table: "Company", newName: "bicswift");

            // Supplier renames
            migrationBuilder.RenameColumn(name: "s_address", table: "Supplier", newName: "address");
            migrationBuilder.RenameColumn(name: "s_postalcode", table: "Supplier", newName: "postalcode");
            migrationBuilder.RenameColumn(name: "s_country", table: "Supplier", newName: "country");
            migrationBuilder.RenameColumn(name: "s_iban", table: "Supplier", newName: "iban");
            migrationBuilder.RenameColumn(name: "s_desc", table: "Supplier", newName: "desc");

            // If plant.supplier_id and acceptance.company_id still need to become string FKs, change types here.
            migrationBuilder.AlterColumn<string>(
                name: "supplier_id",
                table: "Plant",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert column type changes first where applicable
            migrationBuilder.AlterColumn<int>(
                name: "company_id",
                table: "Acceptance",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "supplier_id",
                table: "Plant",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "auctionmaster_id",
                table: "Auction",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            // AuctionMaster revert
            migrationBuilder.RenameColumn(name: "address", table: "AuctionMaster", newName: "am_address");

            // Supplier revert
            migrationBuilder.RenameColumn(name: "desc", table: "Supplier", newName: "s_desc");
            migrationBuilder.RenameColumn(name: "iban", table: "Supplier", newName: "s_iban");
            migrationBuilder.RenameColumn(name: "country", table: "Supplier", newName: "s_country");
            migrationBuilder.RenameColumn(name: "postalcode", table: "Supplier", newName: "s_postalcode");
            migrationBuilder.RenameColumn(name: "address", table: "Supplier", newName: "s_address");

            // Company revert
            migrationBuilder.RenameColumn(name: "bicswift", table: "Company", newName: "c_bicswift");
            migrationBuilder.RenameColumn(name: "iban", table: "Company", newName: "c_iban");
            migrationBuilder.RenameColumn(name: "vat", table: "Company", newName: "c_vat");
            migrationBuilder.RenameColumn(name: "country", table: "Company", newName: "c_country");
            migrationBuilder.RenameColumn(name: "postalcode", table: "Company", newName: "c_postalcode");
            migrationBuilder.RenameColumn(name: "address", table: "Company", newName: "c_address");

            // Plant revert
            migrationBuilder.RenameColumn(name: "desc", table: "Plant", newName: "p_desc");
            migrationBuilder.RenameColumn(name: "maturity", table: "Plant", newName: "p_maturity");
            migrationBuilder.RenameColumn(name: "stems_bunch", table: "Plant", newName: "p_stems_bunch");
            migrationBuilder.RenameColumn(name: "min_stem", table: "Plant", newName: "p_min_stem");
            migrationBuilder.RenameColumn(name: "quality", table: "Plant", newName: "p_quality");
            migrationBuilder.RenameColumn(name: "form", table: "Plant", newName: "p_form");
            migrationBuilder.RenameColumn(name: "category", table: "Plant", newName: "p_category");
            migrationBuilder.RenameColumn(name: "productname", table: "Plant", newName: "p_productname");

            // Media revert
            migrationBuilder.RenameColumn(name: "is_primary", table: "Media", newName: "m_is_primary");
            migrationBuilder.RenameColumn(name: "alt_text", table: "Media", newName: "m_alt_text");
            migrationBuilder.RenameColumn(name: "url", table: "Media", newName: "m_url");

            // AuctionLot revert
            migrationBuilder.RenameColumn(name: "remaining_quantity", table: "AuctionLot", newName: "al_remaining_quantity");
            migrationBuilder.RenameColumn(name: "total_quantity", table: "AuctionLot", newName: "al_total_quantity");
            migrationBuilder.RenameColumn(name: "fustcode", table: "AuctionLot", newName: "al_fustcode");
            migrationBuilder.RenameColumn(name: "min_pickup", table: "AuctionLot", newName: "al_min_pickup");
            migrationBuilder.RenameColumn(name: "containers_in_lot", table: "AuctionLot", newName: "al_containers_in_lot");
            migrationBuilder.RenameColumn(name: "unit_per_container", table: "AuctionLot", newName: "al_unit_per_container");

            // Auction revert renames
            migrationBuilder.RenameColumn(name: "min_price", table: "Auction", newName: "au_min_increment");
            migrationBuilder.RenameColumn(name: "final_price", table: "Auction", newName: "au_final_price");
            migrationBuilder.RenameColumn(name: "start_price", table: "Auction", newName: "au_start_price");
            migrationBuilder.RenameColumn(name: "end_time", table: "Auction", newName: "au_end_time");
            migrationBuilder.RenameColumn(name: "start_time", table: "Auction", newName: "au_start_time");
            migrationBuilder.RenameColumn(name: "status", table: "Auction", newName: "au_status");

            // Acceptance revert
            migrationBuilder.RenameColumn(name: "accepted_quantity", table: "Acceptance", newName: "acc_accepted_quantity");
            migrationBuilder.RenameColumn(name: "accepted_price", table: "Acceptance", newName: "acc_accepted_price");
            migrationBuilder.RenameColumn(name: "tick_number", table: "Acceptance", newName: "acc_tick_number");
            migrationBuilder.RenameColumn(name: "time", table: "Acceptance", newName: "acc_time");
        }
    }
}
