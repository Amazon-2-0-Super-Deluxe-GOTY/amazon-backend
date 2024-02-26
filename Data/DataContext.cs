using amazon_backend.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.Data
{
    public class DataContext : DbContext
    {
        // Entities:public DbSet<Entity.Class> classes{get;set;}
        public DbSet<Category> Categories { get; set; }
<<<<<<< HEAD
        public DbSet<User> Users { get; set; }
        public DbSet<ClientProfile> ClientProfiles { get; set; }
=======
>>>>>>> f5859efca099e9342acc8844a257bef893f07f6e

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
