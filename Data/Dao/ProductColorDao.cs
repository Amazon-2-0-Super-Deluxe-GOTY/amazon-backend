using amazon_backend.Data.Entity;

namespace amazon_backend.Data.Dao
{
    public interface IProductColorDao : IDataAccessObject<ProductColor, Guid>
    {

    }
    public class ProductColorDao : IProductColorDao
    {
        private readonly DataContext dataContext;

        public ProductColorDao(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public void Add(ProductColor item)
        {
            if (item != null)
            {
                dataContext.ProductColors.Add(item);
                dataContext.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            ProductColor? pColor = dataContext.ProductColors.Find(id);
            if (pColor != null)
            {
                dataContext.ProductColors.Remove(pColor);
                dataContext.SaveChanges();
            }
        }

        public ProductColor[] GetAll()
        {
            return dataContext.ProductColors.ToArray();
        }

        public ProductColor? GetById(Guid id)
        {
            ProductColor? pColor = dataContext.ProductColors.Find(id);
            if (pColor != null)
            {
                return pColor;
            }
            return null;
        }

        public void Update(ProductColor item)
        {
            if (item != null)
            {
                dataContext.ProductColors.Update(item);
                dataContext.SaveChanges();
            }
        }
    }
}
