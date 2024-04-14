using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using amazon_backend.Profiles.ProductProfiles;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace amazon_backend.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductDao productDao;
        private readonly IProductPropsDao propsDao;
        private readonly ICategoryDAO categoryDAO;
        private readonly IMapper mapper;
        private const string GUID_PARSE = "Incorrect id";
        private const string CATEGORY_NOT_FOUND = "Category not found";
        private const string PRODUCT_NOT_FOUND = "Product not found";
        private const string CATEGORY_NAME_REQUIRED = "Category name is required";
        public ProductsController(IProductDao productDao, IProductPropsDao propsDao, ICategoryDAO categoryDAO, IMapper mapper)
        {
            this.productDao = productDao;
            this.propsDao = propsDao;
            this.categoryDAO = categoryDAO;
            this.mapper = mapper;
        }
        [HttpGet]
        [Route("products-by-category/{category}")]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return SendResponse(StatusCodes.Status400BadRequest, CATEGORY_NAME_REQUIRED, null);
            }
            Category? _category = await categoryDAO.GetByName(category);
            if (_category == null)
            {
                return SendResponse(StatusCodes.Status404NotFound, CATEGORY_NOT_FOUND, null);
            }

            Product[]? products = await productDao.GetProductsByCategory(_category.Id);

            if (products != null)
            {
                ProductCardProfile[] productCards = mapper.Map<ProductCardProfile[]>(products);
                return SendResponse(StatusCodes.Status200OK, "Ok", productCards);
            }
            return SendResponse(StatusCodes.Status404NotFound, CATEGORY_NOT_FOUND, null);
        }
        [HttpGet]
        [Route("product-by-id/{productId}")]
        public async Task<IActionResult> GetProductById(string productId)
        {
            if (!Guid.TryParse(productId, out var id))
            {
                await Console.Out.WriteLineAsync(id.ToString());
                return SendResponse(StatusCodes.Status400BadRequest, GUID_PARSE, null);
            }
            Product? product = await productDao.GetByIdAsync(id);
            if (product != null)
            {
                try
                {
                    ProductViewProfile productView = mapper.Map<ProductViewProfile>(product);
                    return SendResponse(StatusCodes.Status200OK, "Ok", productView);
                }
                catch(AutoMapperMappingException ex)
                {
                    await Console.Out.WriteLineAsync($"Mapping error: {ex.Message}");
                }
                return SendResponse(StatusCodes.Status200OK, "Ok", product);
            }
            return SendResponse(StatusCodes.Status404NotFound, PRODUCT_NOT_FOUND, null);
        }

        private IActionResult SendResponse(int statusCode, string message, object data, string contentType = "application/json")
        {
            HttpContext.Response.StatusCode = statusCode;
            HttpContext.Response.ContentType = contentType;
            var settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            try
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    Status = statusCode,
                    Message = message,
                    Data = data
                },settings));
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"error json: {ex.Message}");
                return Content(JsonConvert.SerializeObject(new
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "See server logs",
                    Data = (object)null!
                },settings));
            }
        }
    }
}
