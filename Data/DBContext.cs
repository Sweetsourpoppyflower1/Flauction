using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Flauction.Models;

namespace Flauction.Data
{
    public class DBContext : IdentityDbContext<User>
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public DbSet<Flauction.Models.Acceptance> Acceptances { get; set; } = null;
        public DbSet<Flauction.Models.Auction> Auctions { get; set; } = null;
        public DbSet<Flauction.Models.AuctionLot> AuctionLots { get; set; } = null;
        public DbSet<Flauction.Models.AuctionMaster> AuctionMasters { get; set; } = null;
        public DbSet<Flauction.Models.Company> Companies { get; set; } = null;
        public DbSet<Flauction.Models.Media> Medias { get; set; } = null;
        public DbSet<Flauction.Models.Plant> Plants { get; set; } = null;
        public DbSet<Flauction.Models.Supplier> Suppliers { get; set; } = null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Auction -> AuctionMaster
            modelBuilder.Entity<Flauction.Models.Auction>()
                .HasOne<Flauction.Models.AuctionMaster>()
                .WithMany()
                .HasForeignKey(a => a.auctionmaster_id)
                .HasConstraintName("FK_Auction_AuctionMaster")
                .OnDelete(DeleteBehavior.Restrict);

            // Auction -> Plant
            modelBuilder.Entity<Flauction.Models.Auction>()
                .HasOne<Flauction.Models.Plant>()
                .WithMany()
                .HasForeignKey(a => a.plant_id)
                .HasConstraintName("FK_Auction_Plant")
                .OnDelete(DeleteBehavior.Restrict);

            // AuctionLot -> Auction
            modelBuilder.Entity<Flauction.Models.AuctionLot>()
                .HasOne<Flauction.Models.Auction>()
                .WithMany()
                .HasForeignKey(al => al.auction_id)
                .HasConstraintName("FK_AuctionLot_Auction")
                .OnDelete(DeleteBehavior.Restrict);

            // AuctionLot -> Media
            modelBuilder.Entity<Flauction.Models.AuctionLot>()
                .HasOne<Flauction.Models.Media>()
                .WithMany()
                .HasForeignKey(al => al.image_id)
                .HasConstraintName("FK_AuctionLot_Media")
                .OnDelete(DeleteBehavior.Restrict);

            // Acceptance -> Auction
            modelBuilder.Entity<Flauction.Models.Acceptance>()
                .HasOne<Flauction.Models.Auction>()
                .WithMany()
                .HasForeignKey(acc => acc.auction_id)
                .HasConstraintName("FK_Acceptance_Auction")
                .OnDelete(DeleteBehavior.Restrict);

            // Acceptance -> Company
            modelBuilder.Entity<Flauction.Models.Acceptance>()
                .HasOne<Flauction.Models.Company>()
                .WithMany()
                .HasForeignKey(acc => acc.company_id)
                .HasConstraintName("FK_Acceptance_Company")
                .OnDelete(DeleteBehavior.Restrict);

            // Acceptance -> AuctionLot
            modelBuilder.Entity<Flauction.Models.Acceptance>()
                .HasOne<Flauction.Models.AuctionLot>()
                .WithMany()
                .HasForeignKey(acc => acc.auction_lot_id)
                .HasConstraintName("FK_Acceptance_AuctionLot")
                .OnDelete(DeleteBehavior.Restrict);

            // Media -> Plant
            modelBuilder.Entity<Flauction.Models.Media>()
                .HasOne<Flauction.Models.Plant>()
                .WithMany()
                .HasForeignKey(m => m.plant_id)
                .HasConstraintName("FK_Media_Plant")
                .OnDelete(DeleteBehavior.Restrict);

            // Plant -> Supplier
            modelBuilder.Entity<Flauction.Models.Plant>()
                .HasOne<Flauction.Models.Supplier>()
                .WithMany()
                .HasForeignKey(p => p.supplier_id)
                .HasConstraintName("FK_Plant_Supplier")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
