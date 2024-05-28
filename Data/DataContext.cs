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
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewTag> ReviewTags { get; set; }
        public DbSet<ReviewImage> ReviewImages { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItemDao> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<WishListItem> WishListItems { get; set; }
        public DbSet<ProductProperty> ProductProperties { get; set; }
        public DbSet<CategoryPropertyKey> CategoryPropertyKeys { get; set; }
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
                .Property(b => b.IsDeleted)
                .HasDefaultValue(false);
            modelBuilder.Entity<Review>()
                .ToTable(t => t.HasCheckConstraint("ValidMark", "Mark > 0 AND Mark < 6"));
        }
    }
}
