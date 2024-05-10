using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.Data
{
    public class DataContext : DbContext
    {
        // Entities:public DbSet<Entity.Class> classes{get;set;}
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ClientProfile> ClientProfiles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItemDao> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<WishListItem> WishListItems { get; set; }
        public DbSet<ProductProperty> ProductProperties { get; set; }
        public DbSet<CategoryPropertyKey> CategoryPropertyKeys { get; set; }
        public DbSet<AboutProductItem> AboutProductItems { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductRate> ProductRates { get; set; }
        public DbSet<Entity.Token> Tokens { get; set; }
        public DbSet<Entity.TokenJournal> TokenJournals { get; set; }
        public DbSet<Entity.EmailConfirmToken> EmailConfirmTokens { get; set; }

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
            modelBuilder.Entity<ProductRate>()
                .HasKey(pr => new {pr.UserId, pr.ProductId});
            modelBuilder.Entity<ProductRate>()
                .ToTable(t => t.HasCheckConstraint("ValidMark", "Mark > 0 AND Mark < 6"));
            modelBuilder.Entity<Entity.Token>(entity =>
            {
                entity.ToTable("tokens");

                entity.Property(t => t._Token)
                    .IsRequired();

                entity.Property(t => t.ExpirationDate)
                    .IsRequired();
            });
            modelBuilder.Entity<Entity.TokenJournal>(entity =>
            {
                entity.ToTable("token_journals");
                entity.HasKey(ut => ut.Id);

                entity.HasOne(ut => ut.User)
                    .WithMany(u => u.TokenJournals)
                    .HasForeignKey(ut => ut.UserId);

                entity.HasOne(ut => ut.Token)
                    .WithMany(t => t.TokenJournals)
                    .HasForeignKey(ut => ut.TokenId);
            });
        }
    }
}
