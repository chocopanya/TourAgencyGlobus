using Microsoft.EntityFrameworkCore;
using TourAgencyGlobus.Models;

namespace TourAgencyGlobus.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base()
        {
        }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<BusType> BusTypes { get; set; }
        public DbSet<TourApplication> Applications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TourAgencyDB;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tour
            modelBuilder.Entity<Tour>(entity =>
            {
                entity.ToTable("Tours");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("TourID");

                entity.Property(t => t.Price)
                    .HasColumnType("decimal(10,2)");

                entity.Property(t => t.Discount)
                    .HasColumnType("decimal(5,2)");
            });

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("UserID");
            });

            // Country
            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Countries");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("CountryID");
            });

            // BusType
            modelBuilder.Entity<BusType>(entity =>
            {
                entity.ToTable("BusTypes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("BusTypeID");
            });

            // TourApplication
            modelBuilder.Entity<TourApplication>(entity =>
            {
                entity.ToTable("Applications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ApplicationID");

                entity.Property(a => a.TotalCost)
                    .HasColumnType("decimal(10,2)");
            });
        }
    }
}