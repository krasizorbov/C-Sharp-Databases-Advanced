using Microsoft.EntityFrameworkCore;
using P03_SalesDatabase.Data.Models;
using System;

namespace P03_SalesDatabase.Data
{
    public class SalesContext : DbContext
    {
        public SalesContext()
        {

        }
        public SalesContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("Sales");

                entity.HasKey(e => e.SaleId);

                entity.Property(e => e.Date).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Customer).WithMany(c => c.Sales)
                .HasForeignKey( e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Sales_Customers");

                entity.HasOne(e => e.Product).WithMany(p => p.Sales)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Sales_Products");

                entity.HasOne(e => e.Store).WithMany(s => s.Sales)
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Sales_Stores");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("Stores");

                entity.HasKey(e => e.StoreId);

                entity.Property(e => e.Name).HasMaxLength(80).IsUnicode(true).IsRequired();
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customres");

                entity.HasKey(e => e.CustomerId);

                entity.Property(e => e.Name).HasMaxLength(100).IsUnicode(true).IsRequired();

                entity.Property(e => e.Email).HasMaxLength(80).IsUnicode(false).IsRequired();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.Name).HasMaxLength(50).IsUnicode(true).IsRequired();

                entity.Property(e => e.Description).HasMaxLength(250).HasDefaultValue("No description");
            });
        }
    }
}
