using Flauction.Models;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Data
{
    public class DBContext : DbContext 
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Gebruiker> Gebruikers { get; set; } = null!;
        public DbSet<Veiling> Veilingen { get; set; } = null!;
        public DbSet<Veilingsproduct> Veilingsproducten { get; set; } = null!;
        public DbSet<Bod> Biedingen { get; set; } = null!;
        public DbSet<Dashboard> Dashboards { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Veiling>()
                .HasOne(v => v.Veilingmeester)
                .WithMany(g => g.Veilingen)
                .HasForeignKey(v => v.VeilingmeesterID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Veilingsproduct>()
                .HasOne(vp => vp.Veiling)
                .WithMany(v => v.Veilingsproducten)
                .HasForeignKey(vp => vp.VeilingsID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Bod>()
                .HasOne(b => b.Gebruiker)
                .WithMany(g => g.Biedingen)
                .HasForeignKey(b => b.GebruikersID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bod>()
                .HasOne(b => b.Veilingsproduct)
                .WithMany(vp => vp.Biedingen)
                .HasForeignKey(b => b.VeilingsproductID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Dashboard>()
                .HasOne(d => d.Veiling)
                .WithMany(v => v.Dashboards)
                .HasForeignKey(d => d.VeilingsID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
