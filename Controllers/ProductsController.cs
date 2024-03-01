using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [Route("products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductDao productDao;
        private readonly ProductPropsDao propsDao;
        public ProductsController(ProductDao productDao, ProductPropsDao propsDao)
        {
            this.productDao = productDao;
            this.propsDao = propsDao;
        }
        [HttpGet]
        public Product[] GetProducts()
        {
            return productDao.GetAll();
        }
        [HttpPost]
        public Product CreateProduct()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                Name = "Mouse Logitech",
                Brand = "Logitech",
                Price = 250.0,
                DiscountPrice = 0,
                CreatedAt = DateTime.Now,
                ImageUrl = "./mouse.png"
            };
            productDao.Add(product);
            return product;
        }
        [HttpGet]
        [Route("{id}")]
        public Results<NotFound, Ok<Product>> GetProductById(string id)
        {
            Guid productId;
            try
            {
                productId = Guid.Parse(id);
            }
            catch
            {
                return TypedResults.NotFound();
            }
            Product? product = productDao.GetById(productId);
            if (product is not null) return TypedResults.Ok(product);
            return TypedResults.NotFound();
        }
        [HttpGet]
        [Route("/brand/{brand}")]
        public Results<NotFound, Ok<Product[]>> GetProductByBrand(string brand)
        {
            if (!string.IsNullOrEmpty(brand))
            {
                Product[] products = productDao.GetProductsByBrand(brand);
                if (products is not null && products.Length != 0) return TypedResults.Ok(products);
            }
            return TypedResults.NotFound();
        }
        [HttpGet]
        [Route("/product-images/{id}")]
        public Results<NotFound, Ok<ProductImage[]>> GetProductImages(string id)
        {
            Guid productId;
            try
            {
                productId = Guid.Parse(id);
            }
            catch
            {
                return TypedResults.NotFound();
            }
            Product? product = productDao.GetById(productId);
            if (product is not null) return TypedResults.Ok(product.productImages.ToArray());
            return TypedResults.NotFound();
        }
        [HttpDelete]
        [Route("/delete-product/{id}")]
        public IActionResult DeleteProduct(string id)
        {
            Guid productId;
            try
            {
                productId = Guid.Parse(id);
            }
            catch
            {
                return StatusCode(500);
            }
            productDao.Delete(productId);
            return Ok();
        }
        [HttpPut]
        [Route("/restore-product/{id}")]
        public IActionResult RestoreProduct(string id)
        {
            Guid productId;
            try
            {
                productId = Guid.Parse(id);
            }
            catch
            {
                return StatusCode(500);
            }
            productDao.Restore(productId);
            return Ok();
        }
        [HttpGet]
        [Route("/product/props/{id}")]
        public Results<NotFound, Ok<ProductProperty[]>> GetProdPropsByProductId(string id)
        {
            Guid productId;
            try
            {
                productId = Guid.Parse(id);
            }
            catch
            {
                return TypedResults.NotFound();
            }
            var props = propsDao.GetAllProductPropsByProductId(productId);
            if (props is not null) return TypedResults.Ok(props);
            return TypedResults.NotFound();
        }
    }
}
