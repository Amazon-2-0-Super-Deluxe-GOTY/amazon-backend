using amazon_backend.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.Data
{
    public class DataContext : DbContext
    {
        // Entities:public DbSet<Entity.Class> classes{get;set;}
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<WishListItem> WishListItems { get; set; }
        public DbSet<Order> Orders { get; set; }

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
        }
    }
}
