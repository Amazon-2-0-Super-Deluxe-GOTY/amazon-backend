using Microsoft.EntityFrameworkCore;

namespace amazon_backend.Data
{
    public class DataContext:DbContext
    {
        // Entities: DbSet<Entity.Class> classes{get;set;}

        public DataContext(DbContextOptions options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // relations
            // seeds
        }
    }
}
