using amazon_backend.Data.Entity;

namespace amazon_backend.Data.Dao
{
    public interface IProductPropertyDao : IDataAccessObject<ProductProperty, Guid>
    {

    }
    public class ProductPropertyDao : IProductPropertyDao
    {
        private readonly DataContext dataContext;

        public ProductPropertyDao(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Add(ProductProperty item)
        {
            if (item != null)
            {
                dataContext.ProductProperties.Add(item);
                dataContext.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            ProductProperty? pProp = dataContext.ProductProperties.Find(id);
            if (pProp != null)
            {
                dataContext.ProductProperties.Remove(pProp);
                dataContext.SaveChanges();
            }
        }

        public ProductProperty[] GetAll()
        {
            return dataContext.ProductProperties.ToArray();
        }

        public ProductProperty? GetById(Guid id)
        {
            ProductProperty? pProp = dataContext.ProductProperties.Find(id);
            if (pProp != null)
            {
                return pProp;
            }
            return null;
        }

        public void Update(ProductProperty item)
        {
            if(item!=null)
            {
                dataContext.ProductProperties.Update(item);
                dataContext.SaveChanges();
            }
        }
    }
}
