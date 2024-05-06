using amazon_backend.Data;
using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using amazon_backend.Data.Model;
using amazon_backend.Profiles.ProductProfiles;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Text.Json.Nodes;

namespace amazon_backend.Controllers
{
    [Route("api/filter")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly ICategoryDAO categoryDao;
        private readonly IProductDao productDao;
        private readonly IMapper mapper;
        private readonly DataContext dataContext;
        private const string GUID_PARSE = "Incorrect id";
        private const string CATEGORY_NOT_FOUND = "Category not found";
        private const string FILTERS_NOT_FOUND = "Filters not found";
        private const string CATEGORY_NAME_REQUIRED = "Category name is required";
        public FilterController(ICategoryDAO categoryDao, IProductDao productDao, DataContext dataContext, IMapper mapper)
        {
            this.categoryDao = categoryDao;
            this.productDao = productDao;
            this.dataContext = dataContext;
            this.mapper = mapper;
        }
        [HttpGet]
        [Route("filter-items/{category_name}")]
        public async Task<IActionResult> GetFilterItems(string category_name)
        {
            if (string.IsNullOrEmpty(category_name))
            {
                return SendResponse(StatusCodes.Status400BadRequest, CATEGORY_NAME_REQUIRED, null);
            }
            Category? category = await categoryDao.GetByName(category_name);
            if (category == null)
            {
                return SendResponse(StatusCodes.Status404NotFound, CATEGORY_NOT_FOUND, null);
            }
            FilterItemModel[]? filterItems = await categoryDao.GetFilterItems(category.Id);
            if (filterItems != null)
            {
                return SendResponse(StatusCodes.Status200OK, "Ok", filterItems);
            }
            return SendResponse(StatusCodes.Status200OK, FILTERS_NOT_FOUND, null);
        }
        [HttpPost]
        public async Task<IActionResult> FilterProducts([FromBody] ProductFilterBody filterBody)
        {
            // TODO: add body validation;
            if (filterBody != null)
            {
                var category = await categoryDao.GetByName(filterBody.categoryName);
                if(category==null)
                {
                    return SendResponse(404, CATEGORY_NOT_FOUND, null);
                }
                var products = await dataContext.Products.Include(p => p.pProps).Where(p => p.CategoryId == category.Id).ToListAsync();

                if (filterBody.filterProperties != null && filterBody.filterProperties.Count != 0)
                {
                    List<Product> resultProducts = new();
                    foreach(var fItem in filterBody.filterProperties)
                    {
                        foreach(var prod in products)
                        {
                            if (prod.pProps != null)
                            {
                                foreach(var pp in prod.pProps)
                                {
                                    if (pp.Key == fItem.Key && fItem.Value.Contains(pp.Value))
                                    {
                                        if (!resultProducts.Contains(prod))
                                        {
                                            resultProducts.Add(prod);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    List<ProductCardProfile> prodView = mapper.Map<List<ProductCardProfile>>(resultProducts
                        .Skip((filterBody.pageIndex - 1) * filterBody.pageSize)
                        .Take(filterBody.pageSize));
                    return SendResponse(200, "Ok", prodView);
                }
            }
            return SendResponse(400, "Bad request", null);
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
                }, settings));
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"error json: {ex.Message}");
                return Content(JsonConvert.SerializeObject(new
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "See server logs",
                    Data = (object)null!
                }, settings));
            }
        }
    }
}
