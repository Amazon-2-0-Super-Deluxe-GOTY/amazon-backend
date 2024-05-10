using amazon_backend.CQRS.Queries.Request;
using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Services.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace amazon_backend.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IValidator<GetProductsByCategoryQueryRequest> prodByCategoryValidator;
        private readonly IValidator<GetProductByIdQueryRequest> productByIdValidator;
        public ProductsController(IMediator mediator, IValidator<GetProductsByCategoryQueryRequest> validator, IValidator<GetProductByIdQueryRequest> productByIdValidator)
        {
            this.mediator = mediator;
            this.prodByCategoryValidator = validator;
            this.productByIdValidator = productByIdValidator;
        }
        [HttpGet]
        [Route("products-by-category")]
        public async Task<IActionResult> GetProductsByCategory([FromQuery] GetProductsByCategoryQueryRequest request)
        {
            var validationErrors = prodByCategoryValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return SendResponse(StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await mediator.Send(request);
            if (response.IsSuccess)
            {
                List<ProductCardProfile>? productCards = response.Data;
                if (productCards != null)
                {
                    return SendResponse(StatusCodes.Status200OK, "Ok", productCards);
                }
            }
            return SendResponse(StatusCodes.Status404NotFound, response.message, null);
        }
        [HttpGet]
        [Route("product-by-id")]
        public async Task<IActionResult> GetProductById([FromQuery] GetProductByIdQueryRequest request)
        {
            var validationErrors = productByIdValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return SendResponse(StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await mediator.Send(request);
            if (response.IsSuccess)
            {
                return SendResponse(StatusCodes.Status200OK, "Ok", response.Data);
            }
            return SendResponse(StatusCodes.Status404NotFound, response.message, null);
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
