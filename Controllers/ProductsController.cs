using amazon_backend.CQRS.Queries.Request.ProductRequests;
using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Services.FluentValidation;
using amazon_backend.Services.Response;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly RestResponseService _responseService;
        private readonly IMediator mediator;
        private readonly IValidator<GetProductsQueryRequest> prodByCategoryValidator;
        private readonly IValidator<GetProductByIdQueryRequest> productByIdValidator;
        public ProductsController(IMediator mediator, IValidator<GetProductsQueryRequest> validator, IValidator<GetProductByIdQueryRequest> productByIdValidator, RestResponseService responseService)
        {
            this.mediator = mediator;
            this.prodByCategoryValidator = validator;
            this.productByIdValidator = productByIdValidator;
            _responseService = responseService;
        }
        [HttpGet]
        [Route("products-by-category")]
        public async Task<IActionResult> GetProductsByCategory([FromQuery] GetProductsQueryRequest request)
        {
            var validationErrors = prodByCategoryValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await mediator.Send(request);
            if (response.isSuccess)
            {
                List<ProductCardProfile>? productCards = response.data;
                if (productCards != null)
                {
                    return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", productCards,
                        new() { currentPage = request.pageIndex, pagesCount = response.pagesCount });
                }
            }
            return _responseService.SendResponse(HttpContext, StatusCodes.Status404NotFound, response.message, null);
        }
        [HttpGet]
        [Route("product-by-id")]
        public async Task<IActionResult> GetProductById([FromQuery] GetProductByIdQueryRequest request)
        {
            var validationErrors = productByIdValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await mediator.Send(request);
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data);
            }
            return _responseService.SendResponse(HttpContext, StatusCodes.Status404NotFound, response.message, null);
        }
    }
}
