using amazon_backend.Data.Entity;

namespace amazon_backend.Data.Dao
{
    public interface IProductPropsDao:IDataAccessObject<ProductProperty, Guid>
    {
        public ProductProperty[] GetAllProductPropsByProductId(Guid productId);
    }
    public class ProductPropsDao : IProductPropsDao
    {
        private readonly DataContext _context;

        public ProductPropsDao(DataContext context)
        {
            _context = context;
        }

        public void Add(ProductProperty item)
        {
            if (item != null)
            {
                _context.ProductProperties.Add(item);
                _context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            ProductProperty? prop = _context.ProductProperties.Find(id);
            if (prop != null)
            {
                _context.ProductProperties.Remove(prop);
                _context.SaveChanges();
            }
        }

        public ProductProperty[] GetAll()
        {
            if (_context.ProductProperties.Count() != 0)
            {
                return _context.ProductProperties.ToArray();
            }
            return null;
        }

        public ProductProperty[] GetAllProductPropsByProductId(Guid productId)
        {
            var props = _context.ProductProperties.Where(pp => pp.ProductId == productId).ToArray();
            if (props.Length != 0) return props;
            return null;
        }

        public ProductProperty? GetById(Guid id)
        {
            ProductProperty? pProp = _context.ProductProperties.Find(id);
            if (pProp is not null) return pProp;
            return null;
        }

        public void Update(ProductProperty item)
        {
            if (item != null)
            {
                _context.ProductProperties.Update(item);
                _context.SaveChanges();
            }
        }
    }
}
