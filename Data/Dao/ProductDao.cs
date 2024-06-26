﻿using amazon_backend.Data.Entity;
using amazon_backend.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace amazon_backend.Data.Dao
{
    public interface IProductDao : IDataAccessObject<Product, Guid>
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<List<Product>> GetProductsByCategory(uint categoryId);
        ProductImage[] GetProductImages(Guid productId);
        Task<Product[]> GetProductsByCategoryLimit(uint categoryId, int pageSize, int pageIndex);
        void Restore(Guid id);
    }
    public class ProductDao : IProductDao
    {
        private readonly DataContext _context;
        public ProductDao(DataContext context)
        {
            _context = context;
        }
        private bool DbIsConnect()
        {
            if (_context.Database.CanConnect()) return true;
            return false;
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

        public Product? GetById(Guid id)
        {
            return _context.Products.Find(id);
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            if (_context.CanConnect)
            {
                var product = await _context.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.Category)
                    .Include(p => p.ProductProperties)
                    .Include(p => p.AboutProductItems)
                    .Include(p => p.Reviews)
                    .Where(p => p.Id == id)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync();

                if (product != null)
                {
                    return product;
                }
            }
            return null;
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

        public async Task<List<Product>> GetProductsByCategory(uint categoryId)
        {
            if (DbIsConnect())
            {
                var products = await _context.Products
                    .Include(p => p.Reviews)
                    .Where(p => p.CategoryId == categoryId).ToListAsync();
                if (products != null) return products;
            }
            return null;
        }
        public async Task<Product[]> GetProductsByCategoryLimit(uint categoryId, int pageSize, int pageIndex)
        {
            if (DbIsConnect())
            {
                var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();
                if (products != null)
                {
                    return products;
                }
            }
            return null;
        }

        public ProductImage[] GetProductImages(Guid productId)
        {
            if (DbIsConnect())
            {
                Product? product = _context.Products.Include(p => p.ProductImages).Where(p => p.Id == productId).FirstOrDefault();
                if (product != null)
                {
                    var images = product.ProductImages;
                    if (images != null)
                    {
                        return images.ToArray();
                    }
                }
            }
            return null!;
        }

        public static double GetDiscountPrice(Product item)
        {
            if (item == null) return 0;
            var price = item.Price * (1 - (item.DiscountPercent.HasValue ? item.DiscountPercent.Value : 0)) / 100.0;
            return price;
        }
    }
}
