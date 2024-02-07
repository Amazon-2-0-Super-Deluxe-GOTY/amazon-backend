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

        public IEnumerable<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategoryById(int id)
        {
            return _context.Categories.Find(id);
        }

        public Category[] GetCategoriesByName(string name)
        {
            return _context.Categories.Where(c => c.Name.Contains(name)).ToArray();
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void AddSubcategory(int id_parent_category, int id_child_category)
        {
            var parent_category = _context.Categories.Find(id_parent_category);
            var child_category = _context.Categories.Find(id_child_category);
            if (parent_category != null && child_category != null)
            {
                parent_category.ParentCategoryId = child_category.Id;
                _context.SaveChanges();
            }
        }
        public void AddSubcategory(int id_parent_category, Category child_category)
        {
            var parent_category = _context.Categories.Find(id_parent_category);
            if (parent_category != null && child_category != null)
            {
                parent_category.ParentCategory = child_category;
                _context.SaveChanges();
            }
        }

        public void ChangeNameCategoryById(int id, string new_name)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                category.Name = new_name;
                _context.SaveChanges();
            }
        }

        public void ChangeDescriptionCategoryById(int id, string new_description)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                category.Description = new_description;
                _context.SaveChanges();
            }
        }

        public void RestoreCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                category.IsDeleted = false;
                _context.SaveChanges();
            }
        }

        public void DeleteCategory(int id)
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
