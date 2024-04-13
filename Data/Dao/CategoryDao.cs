using amazon_backend.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.Data.Dao
{
    public interface ICategoryDAO : IDataAccessObject<Category, uint>
    {
        Task<Category> GetByName(string name);
        void Restore(uint id);
    }

    public class CategoryDao : ICategoryDAO
    {
        private readonly DataContext _context;

        public CategoryDao(DataContext context)
        {
            _context = context;
        }

        public Category[] GetAll()
        {
            return _context.Categories.ToArray();
        }

        public Category? GetById(uint id)
        {
            return _context.Categories.Find(id);
        }

        public async Task<Category> GetByName(string name)
        {
            var category = await _context.Categories.Where(c => c.Name.Contains(name.ToLower())).FirstOrDefaultAsync();
            if (category != null)
            {
                return category;
            }
            return null;
        }

        public void Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        public void Restore(uint id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                category.IsDeleted = false;
                _context.SaveChanges();
            }
        }

        public void Delete(uint id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                category.IsDeleted = true;
                _context.SaveChanges();
            }
        }
    }
}
