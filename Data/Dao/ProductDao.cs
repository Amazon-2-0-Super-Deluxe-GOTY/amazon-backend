using amazon_backend.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.Data.Dao
{
    public interface IProductDao : IDataAccessObject<Product, Guid>
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product[]> GetProductsByCategory(uint categoryId);
        Task<Product[]> GetProductsByBrand(string brand);
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

        public async Task<Product[]> GetProductsByBrand(string brand)
        {
            if (!string.IsNullOrEmpty(brand))
            {
                try
                {
                    return await _context.Products
                        .Where(p => p.Brand.Contains(brand)).ToArrayAsync();
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

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.productImages)
                .Include(p => p.Category)
                .Include(p => p.pProps)
                .Include(p => p.AboutProductItems)
                .Include(p => p.ProductColors)
                .Where(p => p.Id == id)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
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

        public async Task<Product[]> GetProductsByCategory(uint categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId).ToArrayAsync();
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
