using amazon_backend.Data.Entity;

namespace amazon_backend.Data.Dao
{
    public interface IProductDao : IDataAccessObject<Product, Guid>
    {
        Product[] GetProductsByCategory(Guid categoryId);
        Product[] GetProductsByBrand(string brand);
        Product[] GetProductsByBrandAndCategory(Guid catregoryId,string brand);
        ProductImage[] GetProductImages(Guid productId);
        void Restore(Guid id);
    }
    public class ProductDao : IProductDao
    {
        private readonly DataContext _context;
        public ProductDao(DataContext context)
        {
            _context = context;
        }

        public void Add(Product product)
        {
            if (product != null)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            // TODO: add delete rule for Products
            // TODO: remove product from db. Don't mark as delete
            Product? prod = _context.Products.Find(id);
            if (prod != null)
            {
                prod.DeletedAt = DateTime.Now;
                _context.SaveChanges();
            }

        }

        public Product[] GetAll()
        {
            if (_context.Products.Count() != 0)
            {
                return _context.Products.ToArray();
            }
            else return null!;
        }

        public Product[] GetProductsByBrand(string brand)
        {
            if (!string.IsNullOrEmpty(brand))
            {
                try
                {
                    return _context.Products
                        .Where(p => p.Brand.Contains(brand)).ToArray();
                }
                catch (ArgumentNullException ex)
                {
                    // TODO: ProductDao logger
                    return null!;
                }
            }
            return null!;
        }

        public Product? GetById(Guid id)
        {
            return _context.Products.Find(id);
        }

        public void Update(Product product)
        {
            if (product != null)
            {
                _context.Products.Update(product);
                _context.SaveChanges();
            }
        }

        public void Restore(Guid id)
        {
            Product? prod = _context.Products.Find(id);
            if (prod != null)
            {
                prod.DeletedAt = null;
                _context.SaveChanges();
            }
        }

        public Product[] GetProductsByCategory(Guid categoryId)
        {
            return _context.Products
                 .Where(p => p.CategoryId == categoryId).ToArray();
        }

        public Product[] GetProductsByBrandAndCategory(Guid catregoryId, string brand)
        {
            if (!string.IsNullOrEmpty(brand))
            {
                return _context.Products
                    .Where(p => p.CategoryId == catregoryId && p.Brand.Contains(brand)).ToArray();
            }
            return null!;
        }

        public ProductImage[] GetProductImages(Guid productId)
        {
            Product? product = _context.Products.Find(productId);
            if (product != null)
            {
                return product.productImages.ToArray();
            }
            return null!;
        }
    }
}
