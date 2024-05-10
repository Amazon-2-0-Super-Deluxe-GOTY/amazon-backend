using amazon_backend.CQRS.Queries.Request;
using amazon_backend.Data;
using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using amazon_backend.Data.Model;
using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Services.FluentValidation;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace amazon_backend.Controllers
{
    [Route("api/filter")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IValidator<GetFilterItemsQueryRequest> getFiltItemsValidator;
        public FilterController(IValidator<GetFilterItemsQueryRequest> getFiltItemsValidator, IMediator mediator)
        {
            this.getFiltItemsValidator = getFiltItemsValidator;
            this.mediator = mediator;
        }
        [HttpGet]
        [Route("filter-items")]
        public async Task<IActionResult> GetFilterItems([FromQuery]GetFilterItemsQueryRequest request)
        {
            var validationErrors = getFiltItemsValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return SendResponse(StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await mediator.Send(request);
            if (!response.IsSuccess)
            {
                return SendResponse(StatusCodes.Status404NotFound, response.message, null);
            }
            return SendResponse(StatusCodes.Status200OK, "Ok", response.Data);
        }
        //[HttpPost]
        //public async Task<IActionResult> FilterProducts([FromBody] ProductFilterBody filterBody)
        //{
        //    if (filterBody != null)
        //    {
        //        var category = await categoryDao.GetByName(filterBody.categoryName);
        //        if(category==null)
        //        {
        //            return SendResponse(404, CATEGORY_NOT_FOUND, null);
        //        }
        //        var products = await dataContext.Products.Include(p => p.pProps).Where(p => p.CategoryId == category.Id).ToListAsync();

        //        if (filterBody.filterProperties != null && filterBody.filterProperties.Count != 0)
        //        {
        //            List<Product> resultProducts = new();
        //            foreach(var fItem in filterBody.filterProperties)
        //            {
        //                foreach(var prod in products)
        //                {
        //                    if (prod.pProps != null)
        //                    {
        //                        foreach(var pp in prod.pProps)
        //                        {
        //                            if (pp.Key == fItem.Key && fItem.Value.Contains(pp.Value))
        //                            {
        //                                if (!resultProducts.Contains(prod))
        //                                {
        //                                    resultProducts.Add(prod);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            List<ProductCardProfile> prodView = mapper.Map<List<ProductCardProfile>>(resultProducts
        //                .Skip((filterBody.pageIndex - 1) * filterBody.pageSize)
        //                .Take(filterBody.pageSize));
        //            return SendResponse(200, "Ok", prodView);
        //        }
        //    }
        //    return SendResponse(400, "Bad request", null);
        //}
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
