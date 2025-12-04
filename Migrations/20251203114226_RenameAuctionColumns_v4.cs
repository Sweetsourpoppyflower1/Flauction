using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flauction.Migrations
{
    /// <inheritdoc />
    public partial class RenameAuctionColumns_v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use guarded sp_rename calls to avoid ambiguous @objname / wrong objtype errors
            migrationBuilder.Sql(@"
-- Acceptance
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'acc_time' AND Object_ID = OBJECT_ID(N'dbo.Acceptance'))
    EXEC sp_rename N'dbo.Acceptance.acc_time', N'time', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'acc_tick_number' AND Object_ID = OBJECT_ID(N'dbo.Acceptance'))
    EXEC sp_rename N'dbo.Acceptance.acc_tick_number', N'tick_number', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'acc_accepted_price' AND Object_ID = OBJECT_ID(N'dbo.Acceptance'))
    EXEC sp_rename N'dbo.Acceptance.acc_accepted_price', N'accepted_price', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'acc_accepted_quantity' AND Object_ID = OBJECT_ID(N'dbo.Acceptance'))
    EXEC sp_rename N'dbo.Acceptance.acc_accepted_quantity', N'accepted_quantity', N'COLUMN';

-- Auction
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'au_status' AND Object_ID = OBJECT_ID(N'dbo.Auction'))
    EXEC sp_rename N'dbo.Auction.au_status', N'status', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'au_start_time' AND Object_ID = OBJECT_ID(N'dbo.Auction'))
    EXEC sp_rename N'dbo.Auction.au_start_time', N'start_time', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'au_end_time' AND Object_ID = OBJECT_ID(N'dbo.Auction'))
    EXEC sp_rename N'dbo.Auction.au_end_time', N'end_time', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'au_start_price' AND Object_ID = OBJECT_ID(N'dbo.Auction'))
    EXEC sp_rename N'dbo.Auction.au_start_price', N'start_price', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'au_final_price' AND Object_ID = OBJECT_ID(N'dbo.Auction'))
    EXEC sp_rename N'dbo.Auction.au_final_price', N'final_price', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'au_min_increment' AND Object_ID = OBJECT_ID(N'dbo.Auction'))
    EXEC sp_rename N'dbo.Auction.au_min_increment', N'min_price', N'COLUMN';

-- AuctionLot
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'al_unit_per_container' AND Object_ID = OBJECT_ID(N'dbo.AuctionLot'))
    EXEC sp_rename N'dbo.AuctionLot.al_unit_per_container', N'unit_per_container', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'al_containers_in_lot' AND Object_ID = OBJECT_ID(N'dbo.AuctionLot'))
    EXEC sp_rename N'dbo.AuctionLot.al_containers_in_lot', N'containers_in_lot', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'al_min_pickup' AND Object_ID = OBJECT_ID(N'dbo.AuctionLot'))
    EXEC sp_rename N'dbo.AuctionLot.al_min_pickup', N'min_pickup', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'al_fustcode' AND Object_ID = OBJECT_ID(N'dbo.AuctionLot'))
    EXEC sp_rename N'dbo.AuctionLot.al_fustcode', N'fustcode', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'al_total_quantity' AND Object_ID = OBJECT_ID(N'dbo.AuctionLot'))
    EXEC sp_rename N'dbo.AuctionLot.al_total_quantity', N'total_quantity', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'al_remaining_quantity' AND Object_ID = OBJECT_ID(N'dbo.AuctionLot'))
    EXEC sp_rename N'dbo.AuctionLot.al_remaining_quantity', N'remaining_quantity', N'COLUMN';

-- Media
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'm_url' AND Object_ID = OBJECT_ID(N'dbo.Media'))
    EXEC sp_rename N'dbo.Media.m_url', N'url', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'm_alt_text' AND Object_ID = OBJECT_ID(N'dbo.Media'))
    EXEC sp_rename N'dbo.Media.m_alt_text', N'alt_text', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'm_is_primary' AND Object_ID = OBJECT_ID(N'dbo.Media'))
    EXEC sp_rename N'dbo.Media.m_is_primary', N'is_primary', N'COLUMN';

-- Plant
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'p_productname' AND Object_ID = OBJECT_ID(N'dbo.Plant'))
    EXEC sp_rename N'dbo.Plant.p_productname', N'productname', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'p_category' AND Object_ID = OBJECT_ID(N'dbo.Plant'))
    EXEC sp_rename N'dbo.Plant.p_category', N'category', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'p_form' AND Object_ID = OBJECT_ID(N'dbo.Plant'))
    EXEC sp_rename N'dbo.Plant.p_form', N'form', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'p_quality' AND Object_ID = OBJECT_ID(N'dbo.Plant'))
    EXEC sp_rename N'dbo.Plant.p_quality', N'quality', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'p_min_stem' AND Object_ID = OBJECT_ID(N'dbo.Plant'))
    EXEC sp_rename N'dbo.Plant.p_min_stem', N'min_stem', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'p_stems_bunch' AND Object_ID = OBJECT_ID(N'dbo.Plant'))
    EXEC sp_rename N'dbo.Plant.p_stems_bunch', N'stems_bunch', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'p_maturity' AND Object_ID = OBJECT_ID(N'dbo.Plant'))
    EXEC sp_rename N'dbo.Plant.p_maturity', N'maturity', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'p_desc' AND Object_ID = OBJECT_ID(N'dbo.Plant'))
    EXEC sp_rename N'dbo.Plant.p_desc', N'desc', N'COLUMN';

-- Company
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'c_address' AND Object_ID = OBJECT_ID(N'dbo.Company'))
    EXEC sp_rename N'dbo.Company.c_address', N'address', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'c_postalcode' AND Object_ID = OBJECT_ID(N'dbo.Company'))
    EXEC sp_rename N'dbo.Company.c_postalcode', N'postalcode', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'c_country' AND Object_ID = OBJECT_ID(N'dbo.Company'))
    EXEC sp_rename N'dbo.Company.c_country', N'country', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'c_vat' AND Object_ID = OBJECT_ID(N'dbo.Company'))
    EXEC sp_rename N'dbo.Company.c_vat', N'vat', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'c_iban' AND Object_ID = OBJECT_ID(N'dbo.Company'))
    EXEC sp_rename N'dbo.Company.c_iban', N'iban', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'c_bicswift' AND Object_ID = OBJECT_ID(N'dbo.Company'))
    EXEC sp_rename N'dbo.Company.c_bicswift', N'bicswift', N'COLUMN';

-- Supplier
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N's_address' AND Object_ID = OBJECT_ID(N'dbo.Supplier'))
    EXEC sp_rename N'dbo.Supplier.s_address', N'address', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N's_postalcode' AND Object_ID = OBJECT_ID(N'dbo.Supplier'))
    EXEC sp_rename N'dbo.Supplier.s_postalcode', N'postalcode', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N's_country' AND Object_ID = OBJECT_ID(N'dbo.Supplier'))
    EXEC sp_rename N'dbo.Supplier.s_country', N'country', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N's_iban' AND Object_ID = OBJECT_ID(N'dbo.Supplier'))
    EXEC sp_rename N'dbo.Supplier.s_iban', N'iban', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N's_desc' AND Object_ID = OBJECT_ID(N'dbo.Supplier'))
    EXEC sp_rename N'dbo.Supplier.s_desc', N'desc', N'COLUMN';
");

            // Change auctionmaster_id / supplier_id / company_id types only if they are currently int
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'auctionmaster_id' AND Object_ID = OBJECT_ID(N'dbo.Auction') AND system_type_id = 56) -- 56 = int
BEGIN
    ALTER TABLE dbo.Auction ALTER COLUMN auctionmaster_id nvarchar(450) NOT NULL;
END

IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'supplier_id' AND Object_ID = OBJECT_ID(N'dbo.Plant') AND system_type_id = 56)
BEGIN
    ALTER TABLE dbo.Plant ALTER COLUMN supplier_id nvarchar(450) NOT NULL;
END

IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'company_id' AND Object_ID = OBJECT_ID(N'dbo.Acceptance') AND system_type_id = 56)
BEGIN
    ALTER TABLE dbo.Acceptance ALTER COLUMN company_id nvarchar(450) NOT NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- Down: reverse renames where the new column exists
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'time' AND Object_ID = OBJECT_ID(N'dbo.Acceptance'))
    EXEC sp_rename N'dbo.Acceptance.time', N'acc_time', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'tick_number' AND Object_ID = OBJECT_ID(N'dbo.Acceptance'))
    EXEC sp_rename N'dbo.Acceptance.tick_number', N'acc_tick_number', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'accepted_price' AND Object_ID = OBJECT_ID(N'dbo.Acceptance'))
    EXEC sp_rename N'dbo.Acceptance.accepted_price', N'acc_accepted_price', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'accepted_quantity' AND Object_ID = OBJECT_ID(N'dbo.Acceptance'))
    EXEC sp_rename N'dbo.Acceptance.accepted_quantity', N'acc_accepted_quantity', N'COLUMN';

IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'status' AND Object_ID = OBJECT_ID(N'dbo.Auction'))
    EXEC sp_rename N'dbo.Auction.status', N'au_status', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'start_time' AND Object_ID = OBJECT_ID(N'dbo.Auction'))
    EXEC sp_rename N'dbo.Auction.start_time', N'au_start_time', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'end_time' AND Object_ID = OBJECT_ID(N'dbo.Auction'))
    EXEC sp_rename N'dbo.Auction.end_time', N'au_end_time', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'start_price' AND Object_ID = OBJECT_ID(N'dbo.Auction'))
    EXEC sp_rename N'dbo.Auction.start_price', N'au_start_price', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'final_price' AND Object_ID = OBJECT_ID(N'dbo.Auction'))
    EXEC sp_rename N'dbo.Auction.final_price', N'au_final_price', N'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'min_price' AND Object_ID = OBJECT_ID(N'dbo.Auction'))
    EXEC sp_rename N'dbo.Auction.min_price', N'au_min_increment', N'COLUMN';

-- revert type changes if they are nvarchar(450)
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'auctionmaster_id' AND Object_ID = OBJECT_ID(N'dbo.Auction') AND system_type_id = 231) -- 231 = nvarchar
BEGIN
    ALTER TABLE dbo.Auction ALTER COLUMN auctionmaster_id int NOT NULL;
END

IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'supplier_id' AND Object_ID = OBJECT_ID(N'dbo.Plant') AND system_type_id = 231)
BEGIN
    ALTER TABLE dbo.Plant ALTER COLUMN supplier_id int NOT NULL;
END

IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'company_id' AND Object_ID = OBJECT_ID(N'dbo.Acceptance') AND system_type_id = 231)
BEGIN
    ALTER TABLE dbo.Acceptance ALTER COLUMN company_id int NOT NULL;
END
");
        }
    }
}
