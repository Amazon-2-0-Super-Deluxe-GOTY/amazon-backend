using amazon_backend.Data.Entity;

namespace amazon_backend.Data.Dao
{
    public interface ICategoryPropertyDao : IDataAccessObject<CategoryPropertyKey, Guid>
    {

    }
    public class CategoryPropertyKeyDao
    {
        private readonly DataContext _dataContext;

        public CategoryPropertyKeyDao(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async void Add(CategoryPropertyKey key)
        {
            if (key != null)
            {
               await _dataContext.CategoryPropertyKeys.AddAsync(key);
               await _dataContext.SaveChangesAsync();

            }
        }
        public void Delete(Guid id)
        {
            CategoryPropertyKey categoryPropertyKey = _dataContext.CategoryPropertyKeys.FirstOrDefault(x => x.Id == id);
            if (categoryPropertyKey != null)
            {
                _dataContext.CategoryPropertyKeys.Remove(categoryPropertyKey);
                _dataContext.SaveChanges();
            }
        }
        public CategoryPropertyKey[] GetAll()
        {
            return _dataContext.CategoryPropertyKeys.ToArray();
        }
        public CategoryPropertyKey GetById(Guid id)
        {
            CategoryPropertyKey? pProp = _dataContext.CategoryPropertyKeys.Find(id);
            if (pProp != null)
            {
                return pProp;
            }
            return null;
        }
        public void Update(CategoryPropertyKey item)
        {
            if (item != null)
            {
                _dataContext.CategoryPropertyKeys.Update(item);
                _dataContext.SaveChanges();
            }
        }
    }
}
