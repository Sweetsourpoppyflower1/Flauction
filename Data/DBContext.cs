using Microsoft.EntityFrameworkCore;

namespace Flauction.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public DbSet<Flauction.Models.Acceptance> Acceptances { get; set; } = null;
        public DbSet<Flauction.Models.Auction> Auctions { get; set; } = null;
        public DbSet<Flauction.Models.AuctionClock> AuctionClocks { get; set; } = null;
        public DbSet<Flauction.Models.AuctionLot> AuctionLots { get; set; } = null;
        public DbSet<Flauction.Models.AuctionMaster> AuctionMasters { get; set; } = null;
        public DbSet<Flauction.Models.Company> Companies { get; set; } = null;
        public DbSet<Flauction.Models.ContactPerson> ContactPersons { get; set; } = null;
        public DbSet<Flauction.Models.Media> Medias { get; set; } = null;
        public DbSet<Flauction.Models.Plant> Plants { get; set; } = null;
        public DbSet<Flauction.Models.Supplier> Suppliers { get; set; } = null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Flauction.Models.Auction>()
                .HasOne<Flauction.Models.AuctionMaster>()
                .WithMany()
                .HasForeignKey("auctionmaster_id")
                .OnDelete(DeleteBehavior.Restrict);

            // Auction -> Plant
            modelBuilder.Entity<Flauction.Models.Auction>()
                .HasOne<Flauction.Models.Plant>()
                .WithMany()
                .HasForeignKey("plant_id")
                .OnDelete(DeleteBehavior.Restrict);

            // Auction -> Winner Company
            modelBuilder.Entity<Flauction.Models.Auction>()
                .HasOne<Flauction.Models.Company>()
                .WithMany()
                .HasForeignKey("winner_company_id")
                .OnDelete(DeleteBehavior.Restrict);

            // AuctionLot -> Auction
            modelBuilder.Entity<Flauction.Models.AuctionLot>()
                .HasOne<Flauction.Models.Auction>()
                .WithMany()
                .HasForeignKey("auction_id")
                .OnDelete(DeleteBehavior.Restrict);

            // AuctionLot -> Media
            modelBuilder.Entity<Flauction.Models.AuctionLot>()
                .HasOne<Flauction.Models.Media>()
                .WithMany()
                .HasForeignKey("image_id")
                .OnDelete(DeleteBehavior.Restrict);

            // AuctionClock -> Auction
            modelBuilder.Entity<Flauction.Models.AuctionClock>()
                .HasOne<Flauction.Models.Auction>()
                .WithMany()
                .HasForeignKey("auction_id")
                .OnDelete(DeleteBehavior.Restrict);

            // Acceptance -> Auction
            modelBuilder.Entity<Flauction.Models.Acceptance>()
                .HasOne<Flauction.Models.Auction>()
                .WithMany()
                .HasForeignKey("auction_id")
                .OnDelete(DeleteBehavior.Restrict);

            // Acceptance -> Company
            modelBuilder.Entity<Flauction.Models.Acceptance>()
                .HasOne<Flauction.Models.Company>()
                .WithMany()
                .HasForeignKey("company_id")
                .OnDelete(DeleteBehavior.Restrict);

            // Acceptance -> AuctionLot
            modelBuilder.Entity<Flauction.Models.Acceptance>()
                .HasOne<Flauction.Models.AuctionLot>()
                .WithMany()
                .HasForeignKey("auction_lot_id")
                .OnDelete(DeleteBehavior.Restrict);

            // ContactPerson -> Company
            modelBuilder.Entity<Flauction.Models.ContactPerson>()
                .HasOne<Flauction.Models.Company>()
                .WithMany()
                .HasForeignKey("company_id")
                .OnDelete(DeleteBehavior.Restrict);

            // Media -> Plant
            modelBuilder.Entity<Flauction.Models.Media>()
                .HasOne<Flauction.Models.Plant>()
                .WithMany()
                .HasForeignKey("plant_id")
                .OnDelete(DeleteBehavior.Restrict);

            // Plant -> Supplier
            modelBuilder.Entity<Flauction.Models.Plant>()
                .HasOne<Flauction.Models.Supplier>()
                .WithMany()
                .HasForeignKey("supplier_id")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
