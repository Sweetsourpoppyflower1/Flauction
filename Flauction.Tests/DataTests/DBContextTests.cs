using System;
using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Flauction.Tests.DataTests
{
    public class DBContextTests : IDisposable
    {
        private readonly DbContextOptions<DBContext> _options;

        public DBContextTests()
        {
            _options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        public void Dispose()
        {
            using var context = new DBContext(_options);
            context.Database.EnsureDeleted();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidOptions_CreatesInstance()
        {
            // Arrange & Act
            using var context = new DBContext(_options);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Acceptances);
            Assert.NotNull(context.Auctions);
            Assert.NotNull(context.AuctionLots);
            Assert.NotNull(context.AuctionMasters);
            Assert.NotNull(context.Companies);
            Assert.NotNull(context.Medias);
            Assert.NotNull(context.MediaPlants);
            Assert.NotNull(context.Plants);
            Assert.NotNull(context.Suppliers);
        }

        [Fact]
        public void Constructor_WithNullOptions_ThrowsArgumentNullException()
        {
            // Arrange
            DbContextOptions<DBContext> nullOptions = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DBContext(nullOptions));
        }

        #endregion

        #region DbSet Property Tests

        [Fact]
        public void Acceptances_DbSet_IsAccessible()
        {
            // Arrange
            using var context = new DBContext(_options);

            // Act & Assert
            Assert.NotNull(context.Acceptances);
            Assert.IsAssignableFrom<DbSet<Acceptance>>(context.Acceptances);
        }

        [Fact]
        public void Auctions_DbSet_IsAccessible()
        {
            // Arrange
            using var context = new DBContext(_options);

            // Act & Assert
            Assert.NotNull(context.Auctions);
            Assert.IsAssignableFrom<DbSet<Auction>>(context.Auctions);
        }

        [Fact]
        public void AuctionLots_DbSet_IsAccessible()
        {
            // Arrange
            using var context = new DBContext(_options);

            // Act & Assert
            Assert.NotNull(context.AuctionLots);
            Assert.IsAssignableFrom<DbSet<AuctionLot>>(context.AuctionLots);
        }

        [Fact]
        public void AuctionMasters_DbSet_IsAccessible()
        {
            // Arrange
            using var context = new DBContext(_options);

            // Act & Assert
            Assert.NotNull(context.AuctionMasters);
            Assert.IsAssignableFrom<DbSet<AuctionMaster>>(context.AuctionMasters);
        }

        [Fact]
        public void Companies_DbSet_IsAccessible()
        {
            // Arrange
            using var context = new DBContext(_options);

            // Act & Assert
            Assert.NotNull(context.Companies);
            Assert.IsAssignableFrom<DbSet<Company>>(context.Companies);
        }

        [Fact]
        public void Medias_DbSet_IsAccessible()
        {
            // Arrange
            using var context = new DBContext(_options);

            // Act & Assert
            Assert.NotNull(context.Medias);
            Assert.IsAssignableFrom<DbSet<Media>>(context.Medias);
        }

        [Fact]
        public void MediaPlants_DbSet_IsAccessible()
        {
            // Arrange
            using var context = new DBContext(_options);

            // Act & Assert
            Assert.NotNull(context.MediaPlants);
            Assert.IsAssignableFrom<DbSet<MediaPlant>>(context.MediaPlants);
        }

        [Fact]
        public void Plants_DbSet_IsAccessible()
        {
            // Arrange
            using var context = new DBContext(_options);

            // Act & Assert
            Assert.NotNull(context.Plants);
            Assert.IsAssignableFrom<DbSet<Plant>>(context.Plants);
        }

        [Fact]
        public void Suppliers_DbSet_IsAccessible()
        {
            // Arrange
            using var context = new DBContext(_options);

            // Act & Assert
            Assert.NotNull(context.Suppliers);
            Assert.IsAssignableFrom<DbSet<Supplier>>(context.Suppliers);
        }

        #endregion

        #region OnModelCreating - Relationship Configuration Tests

        [Fact]
        public void OnModelCreating_ConfiguresAuctionAuctionMasterRelationship()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var auctionMaster = new AuctionMaster { Id = "am1" };
            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };
            var auction = new Auction
            {
                auction_id = 1,
                auctionmaster_id = "am1",
                plant_id = 1,
                status = "Active",
                start_time = DateTime.Now,
                end_time = DateTime.Now.AddHours(1)
            };

            // Act
            context.AuctionMasters.Add(auctionMaster);
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.Auctions.Add(auction);
            context.SaveChanges();

            // Assert
            var savedAuction = context.Auctions.FirstOrDefault(a => a.auction_id == 1);
            Assert.NotNull(savedAuction);
            Assert.Equal("am1", savedAuction.auctionmaster_id);
        }

        [Fact]
        public void OnModelCreating_ConfiguresAuctionPlantRelationship()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Tulip",
                category = "Flower",
                form = "Cut",
                quality = "Standard",
                min_stem = "40",
                stems_bunch = "5",
                maturity = "Mature",
                start_price = 50,
                min_price = 30
            };
            var auctionMaster = new AuctionMaster { Id = "am1" };
            var auction = new Auction
            {
                auction_id = 1,
                auctionmaster_id = "am1",
                plant_id = 1,
                status = "Active",
                start_time = DateTime.Now,
                end_time = DateTime.Now.AddHours(2)
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.AuctionMasters.Add(auctionMaster);
            context.Auctions.Add(auction);
            context.SaveChanges();

            // Assert
            var savedAuction = context.Auctions.FirstOrDefault(a => a.auction_id == 1);
            Assert.NotNull(savedAuction);
            Assert.Equal(1, savedAuction.plant_id);
        }

        [Fact]
        public void OnModelCreating_ConfiguresAcceptanceAuctionRelationship()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };
            var auctionMaster = new AuctionMaster { Id = "am1" };
            var auction = new Auction
            {
                auction_id = 1,
                auctionmaster_id = "am1",
                plant_id = 1,
                status = "Active",
                start_time = DateTime.Now,
                end_time = DateTime.Now.AddHours(1)
            };
            var company = new Company { Id = "comp1" };
            var auctionLot = new AuctionLot
            {
                auctionlot_id = 1,
                plant_id = 1,
                unit_per_container = 10,
                containers_in_lot = 5,
                min_pickup = 1,
                start_quantity = 50,
                remaining_quantity = 50
            };
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "comp1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 150,
                accepted_quantity = 50,
                time = DateTime.Now
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.AuctionMasters.Add(auctionMaster);
            context.Auctions.Add(auction);
            context.Companies.Add(company);
            context.AuctionLots.Add(auctionLot);
            context.Acceptances.Add(acceptance);
            context.SaveChanges();

            // Assert
            var savedAcceptance = context.Acceptances.FirstOrDefault(a => a.acceptance_id == 1);
            Assert.NotNull(savedAcceptance);
            Assert.Equal(1, savedAcceptance.auction_id);
        }

        [Fact]
        public void OnModelCreating_ConfiguresAcceptanceCompanyRelationship()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };
            var auctionMaster = new AuctionMaster { Id = "am1" };
            var auction = new Auction
            {
                auction_id = 1,
                auctionmaster_id = "am1",
                plant_id = 1,
                status = "Active",
                start_time = DateTime.Now,
                end_time = DateTime.Now.AddHours(1)
            };
            var company = new Company { Id = "comp1" };
            var auctionLot = new AuctionLot
            {
                auctionlot_id = 1,
                plant_id = 1,
                unit_per_container = 10,
                containers_in_lot = 5,
                min_pickup = 1,
                start_quantity = 50,
                remaining_quantity = 50
            };
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "comp1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 150,
                accepted_quantity = 50,
                time = DateTime.Now
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.AuctionMasters.Add(auctionMaster);
            context.Auctions.Add(auction);
            context.Companies.Add(company);
            context.AuctionLots.Add(auctionLot);
            context.Acceptances.Add(acceptance);
            context.SaveChanges();

            // Assert
            var savedAcceptance = context.Acceptances.FirstOrDefault(a => a.acceptance_id == 1);
            Assert.NotNull(savedAcceptance);
            Assert.Equal("comp1", savedAcceptance.company_id);
        }

        [Fact]
        public void OnModelCreating_ConfiguresAcceptanceAuctionLotRelationship()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };
            var auctionMaster = new AuctionMaster { Id = "am1" };
            var auction = new Auction
            {
                auction_id = 1,
                auctionmaster_id = "am1",
                plant_id = 1,
                status = "Active",
                start_time = DateTime.Now,
                end_time = DateTime.Now.AddHours(1)
            };
            var company = new Company { Id = "comp1" };
            var auctionLot = new AuctionLot
            {
                auctionlot_id = 1,
                plant_id = 1,
                unit_per_container = 10,
                containers_in_lot = 5,
                min_pickup = 1,
                start_quantity = 50,
                remaining_quantity = 50
            };
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "comp1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 150,
                accepted_quantity = 50,
                time = DateTime.Now
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.AuctionMasters.Add(auctionMaster);
            context.Auctions.Add(auction);
            context.Companies.Add(company);
            context.AuctionLots.Add(auctionLot);
            context.Acceptances.Add(acceptance);
            context.SaveChanges();

            // Assert
            var savedAcceptance = context.Acceptances.FirstOrDefault(a => a.acceptance_id == 1);
            Assert.NotNull(savedAcceptance);
            Assert.Equal(1, savedAcceptance.auction_lot_id);
        }

        [Fact]
        public void OnModelCreating_ConfiguresMediaPlantPlantRelationship()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };
            var mediaPlant = new MediaPlant
            {
                mediaplant_id = 1,
                plant_id = 1,
                url = "http://example.com/image.jpg",
                alt_text = "Rose image",
                is_primary = true
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.MediaPlants.Add(mediaPlant);
            context.SaveChanges();

            // Assert
            var savedMediaPlant = context.MediaPlants.FirstOrDefault(m => m.mediaplant_id == 1);
            Assert.NotNull(savedMediaPlant);
            Assert.Equal(1, savedMediaPlant.plant_id);
        }

        [Fact]
        public void OnModelCreating_ConfiguresPlantSupplierRelationship()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.SaveChanges();

            // Assert
            var savedPlant = context.Plants.FirstOrDefault(p => p.plant_id == 1);
            Assert.NotNull(savedPlant);
            Assert.Equal("sup1", savedPlant.supplier_id);
        }

        #endregion

        #region OnModelCreating - Delete Behavior Configuration Tests

        [Fact]
        public void OnModelCreating_Auction_AuctionMaster_ConfiguresCorrectly()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var auctionMaster = new AuctionMaster { Id = "am1" };
            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };
            var auction = new Auction
            {
                auction_id = 1,
                auctionmaster_id = "am1",
                plant_id = 1,
                status = "Active",
                start_time = DateTime.Now,
                end_time = DateTime.Now.AddHours(1)
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.AuctionMasters.Add(auctionMaster);
            context.Auctions.Add(auction);
            context.SaveChanges();

            // Assert
            var savedAuction = context.Auctions.FirstOrDefault(a => a.auction_id == 1);
            Assert.NotNull(savedAuction);
            Assert.Equal("am1", savedAuction.auctionmaster_id);
        }

        [Fact]
        public void OnModelCreating_Auction_Plant_ConfiguresCorrectly()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };
            var auctionMaster = new AuctionMaster { Id = "am1" };
            var auction = new Auction
            {
                auction_id = 1,
                auctionmaster_id = "am1",
                plant_id = 1,
                status = "Active",
                start_time = DateTime.Now,
                end_time = DateTime.Now.AddHours(1)
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.AuctionMasters.Add(auctionMaster);
            context.Auctions.Add(auction);
            context.SaveChanges();

            // Assert
            var savedAuction = context.Auctions.FirstOrDefault(a => a.auction_id == 1);
            Assert.NotNull(savedAuction);
        }

        [Fact]
        public void OnModelCreating_Acceptance_Auction_ConfiguresCorrectly()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };
            var auctionMaster = new AuctionMaster { Id = "am1" };
            var auction = new Auction
            {
                auction_id = 1,
                auctionmaster_id = "am1",
                plant_id = 1,
                status = "Active",
                start_time = DateTime.Now,
                end_time = DateTime.Now.AddHours(1)
            };
            var company = new Company { Id = "comp1" };
            var auctionLot = new AuctionLot
            {
                auctionlot_id = 1,
                plant_id = 1,
                unit_per_container = 10,
                containers_in_lot = 5,
                min_pickup = 1,
                start_quantity = 50,
                remaining_quantity = 50
            };
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "comp1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 150,
                accepted_quantity = 50,
                time = DateTime.Now
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.AuctionMasters.Add(auctionMaster);
            context.Auctions.Add(auction);
            context.Companies.Add(company);
            context.AuctionLots.Add(auctionLot);
            context.Acceptances.Add(acceptance);
            context.SaveChanges();

            // Assert
            var savedAcceptance = context.Acceptances.FirstOrDefault(a => a.acceptance_id == 1);
            Assert.NotNull(savedAcceptance);
        }

        [Fact]
        public void OnModelCreating_Acceptance_Company_ConfiguresCorrectly()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };
            var auctionMaster = new AuctionMaster { Id = "am1" };
            var auction = new Auction
            {
                auction_id = 1,
                auctionmaster_id = "am1",
                plant_id = 1,
                status = "Active",
                start_time = DateTime.Now,
                end_time = DateTime.Now.AddHours(1)
            };
            var company = new Company { Id = "comp1" };
            var auctionLot = new AuctionLot
            {
                auctionlot_id = 1,
                plant_id = 1,
                unit_per_container = 10,
                containers_in_lot = 5,
                min_pickup = 1,
                start_quantity = 50,
                remaining_quantity = 50
            };
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "comp1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 150,
                accepted_quantity = 50,
                time = DateTime.Now
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.AuctionMasters.Add(auctionMaster);
            context.Auctions.Add(auction);
            context.Companies.Add(company);
            context.AuctionLots.Add(auctionLot);
            context.Acceptances.Add(acceptance);
            context.SaveChanges();

            // Assert
            var savedAcceptance = context.Acceptances.FirstOrDefault(a => a.acceptance_id == 1);
            Assert.NotNull(savedAcceptance);
        }

        [Fact]
        public void OnModelCreating_Acceptance_AuctionLot_ConfiguresCorrectly()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };
            var auctionMaster = new AuctionMaster { Id = "am1" };
            var auction = new Auction
            {
                auction_id = 1,
                auctionmaster_id = "am1",
                plant_id = 1,
                status = "Active",
                start_time = DateTime.Now,
                end_time = DateTime.Now.AddHours(1)
            };
            var company = new Company { Id = "comp1" };
            var auctionLot = new AuctionLot
            {
                auctionlot_id = 1,
                plant_id = 1,
                unit_per_container = 10,
                containers_in_lot = 5,
                min_pickup = 1,
                start_quantity = 50,
                remaining_quantity = 50
            };
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "comp1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 150,
                accepted_quantity = 50,
                time = DateTime.Now
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.AuctionMasters.Add(auctionMaster);
            context.Auctions.Add(auction);
            context.Companies.Add(company);
            context.AuctionLots.Add(auctionLot);
            context.Acceptances.Add(acceptance);
            context.SaveChanges();

            // Assert
            var savedAcceptance = context.Acceptances.FirstOrDefault(a => a.acceptance_id == 1);
            Assert.NotNull(savedAcceptance);
        }

        [Fact]
        public void OnModelCreating_MediaPlant_Plant_ConfiguresCorrectly()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };
            var mediaPlant = new MediaPlant
            {
                mediaplant_id = 1,
                plant_id = 1,
                url = "http://example.com/image.jpg",
                alt_text = "Rose image",
                is_primary = true
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.MediaPlants.Add(mediaPlant);
            context.SaveChanges();

            // Assert
            var savedMediaPlant = context.MediaPlants.FirstOrDefault(m => m.mediaplant_id == 1);
            Assert.NotNull(savedMediaPlant);
        }

        [Fact]
        public void OnModelCreating_Plant_Supplier_ConfiguresCorrectly()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.SaveChanges();

            // Assert
            var savedPlant = context.Plants.FirstOrDefault(p => p.plant_id == 1);
            Assert.NotNull(savedPlant);
        }

        #endregion

        #region Inheritance Tests

        [Fact]
        public void DBContext_InheritsFromIdentityDbContext()
        {
            // Arrange
            using var context = new DBContext(_options);

            // Act & Assert
            Assert.IsAssignableFrom<IdentityDbContext>(context);
        }

        [Fact]
        public void Users_DbSet_IsAvailableFromIdentityDbContext()
        {
            // Arrange
            using var context = new DBContext(_options);

            // Act & Assert
            Assert.NotNull(context.Users);
        }

        [Fact]
        public void Roles_DbSet_IsAvailableFromIdentityDbContext()
        {
            // Arrange
            using var context = new DBContext(_options);

            // Act & Assert
            Assert.NotNull(context.Roles);
        }

        #endregion

        #region Data Persistence Tests

        [Fact]
        public void SaveChanges_AddsNewEntity_ToDatabase()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };

            // Act
            context.Suppliers.Add(supplier);
            context.SaveChanges();

            // Assert
            var saved = context.Suppliers.FirstOrDefault(s => s.Id == "sup1");
            Assert.NotNull(saved);
            Assert.Equal("sup1", saved.Id);
        }

        [Fact]
        public void SaveChanges_UpdatesExistingEntity_InDatabase()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1", country = "Netherlands" };
            context.Suppliers.Add(supplier);
            context.SaveChanges();

            // Act
            supplier.country = "Belgium";
            context.Suppliers.Update(supplier);
            context.SaveChanges();

            // Assert
            using var newContext = new DBContext(_options);
            var updated = newContext.Suppliers.FirstOrDefault(s => s.Id == "sup1");
            Assert.NotNull(updated);
            Assert.Equal("Belgium", updated.country);
        }

        [Fact]
        public void SaveChanges_DeletesEntity_FromDatabase()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            context.Suppliers.Add(supplier);
            context.SaveChanges();

            // Act
            context.Suppliers.Remove(supplier);
            context.SaveChanges();

            // Assert
            var deleted = context.Suppliers.FirstOrDefault(s => s.Id == "sup1");
            Assert.Null(deleted);
        }

        #endregion

        #region Query Tests

        [Fact]
        public void Context_CanQueryAuctions_WithoutErrors()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            // Act
            var auctions = context.Auctions.ToList();

            // Assert
            Assert.NotNull(auctions);
            Assert.IsType<List<Auction>>(auctions);
        }

        [Fact]
        public void Context_CanQueryMultipleEntities_WithoutErrors()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            // Act
            var suppliers = context.Suppliers.ToList();
            var plants = context.Plants.ToList();
            var companies = context.Companies.ToList();

            // Assert
            Assert.NotNull(suppliers);
            Assert.NotNull(plants);
            Assert.NotNull(companies);
        }

        [Fact]
        public void Context_CanAddAndQueryAuctions()
        {
            // Arrange
            using var context = new DBContext(_options);
            context.Database.EnsureCreated();

            var supplier = new Supplier { Id = "sup1" };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "sup1",
                productname = "Rose",
                category = "Flower",
                form = "Cut",
                quality = "Premium",
                min_stem = "50",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 100,
                min_price = 80
            };
            var auctionMaster = new AuctionMaster { Id = "am1" };
            var auction = new Auction
            {
                auction_id = 1,
                auctionmaster_id = "am1",
                plant_id = 1,
                status = "Active",
                start_time = DateTime.Now,
                end_time = DateTime.Now.AddHours(1)
            };

            // Act
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.AuctionMasters.Add(auctionMaster);
            context.Auctions.Add(auction);
            context.SaveChanges();

            var queriedAuctions = context.Auctions.ToList();

            // Assert
            Assert.NotNull(queriedAuctions);
            Assert.Single(queriedAuctions);
            Assert.Equal("Active", queriedAuctions[0].status);
        }

        #endregion
    }
}