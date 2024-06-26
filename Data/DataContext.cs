﻿using amazon_backend.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.Data
{
    public class DataContext : DbContext
    {
        // Entities:public DbSet<Entity.Class> classes{get;set;}
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewTag> ReviewTags { get; set; }
        public DbSet<ReviewImage> ReviewImages { get; set; }
        public DbSet<ReviewLike> ReviewLikes { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        public DbSet<ProductProperty> ProductProperties { get; set; }
        public DbSet<CategoryPropertyKey> CategoryPropertyKeys { get; set; }
        public DbSet<CategoryImage> CategoryImages { get; set; }
        public DbSet<AboutProductItem> AboutProductItems { get; set; }
        public DbSet<JwtToken> JwtTokens { get; set; }
        public DbSet<TokenJournal> TokenJournals { get; set; }

        public bool CanConnect { get; set; }
        public DataContext(DbContextOptions options) : base(options)
        {
            CanConnect = this.Database.CanConnect();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // relations
            // seeds
            modelBuilder.Entity<Category>()
                .Property(b => b.IsActive)
                .HasDefaultValue(true);
            modelBuilder.Entity<Review>()
                .ToTable(t => t.HasCheckConstraint("ValidMark", "Mark > 0 AND Mark < 6"));
            modelBuilder.Entity<Category>()
                .HasMany(c => c.CategoryPropertyKeys)
                .WithOne(cp => cp.Category)
                .HasForeignKey(cp => cp.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
