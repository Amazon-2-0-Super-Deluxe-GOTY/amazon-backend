﻿using amazon_backend.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.Data
{
    public class DataContext : DbContext
    {
        // Entities:public DbSet<Entity.Class> classes{get;set;}
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ClientProfile> ClientProfiles { get; set; }
        public DbSet<SellerProfile> SellerProfiles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<WishListItem> WishListItems { get; set; }
        public DbSet<ProductProperty> ProductProperties { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // relations
            // seeds
            modelBuilder.Entity<Category>()
                .Property(b => b.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.productImages)
                .HasForeignKey(pi => pi.ProductId)
                .HasPrincipalKey(p => p.Id);

            modelBuilder.Entity<ProductProperty>()
                .HasOne(pp => pp.Product)
                .WithMany(p => p.pProps)
                .HasForeignKey(pp => pp.ProductId)
                .HasPrincipalKey(p => p.Id);
        }
    }
}
