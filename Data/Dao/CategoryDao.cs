using amazon_backend.Data.Entity;

namespace amazon_backend.Data.Dao
{
    public class CategoryDao
    {
        private readonly DataContext _context;

        public CategoryDao(DataContext context)
        {
            _context = context;
        }

        public Category[] GetAllCategories()
        {
            return _context.Categories.ToArray();
        }

        public Category? GetCategoryById(uint id)
        {
            return _context.Categories.Find(id);
        }

        public Category[] GetCategoriesByName(string name)
        {
            return _context.Categories.Where(c => c.Name.Contains(name.ToLower())).ToArray();
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void Update(Category category)
        {
            _context.Update(category);
            _context.SaveChanges();
        }

        public void RestoreCategory(uint id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                category.IsDeleted = false;
                _context.SaveChanges();
            }
        }

        public void DeleteCategory(uint id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                category.IsDeleted = true;
                _context.SaveChanges();
            }
        }

        /*public void SwitchActiveStateCategory(int id)   // alternative to DeleteCategory and RestoreCategory
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                category.IsDeleted = !category.IsDeleted;
                _context.SaveChanges();
            }
        }*/
    }
}
