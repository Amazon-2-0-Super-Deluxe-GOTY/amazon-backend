using amazon_backend.CQRS.Queries.Request.ProductRequests;
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
        private readonly IMediator _mediator;
        private readonly IValidator<GetProductsQueryRequest> _prodByCategoryValidator;
        private readonly IValidator<GetProductByIdQueryRequest> _productByIdValidator;
        private readonly IValidator<GetFilterItemsQueryRequest> _filterItemsValidator;

        public ProductsController(IMediator mediator, IValidator<GetProductsQueryRequest> validator, IValidator<GetProductByIdQueryRequest> productByIdValidator, RestResponseService responseService, IValidator<GetFilterItemsQueryRequest> filterItemsValidator)
        {
            _mediator = mediator;
            _prodByCategoryValidator = validator;
            _productByIdValidator = productByIdValidator;
            _responseService = responseService;
            _filterItemsValidator = filterItemsValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductsQueryRequest request)
        {
            var validationErrors = _prodByCategoryValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data,
                    new() { currentPage = request.pageIndex, pagesCount = response.pagesCount });
            }
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpGet]
        [Route("byId")]
        public async Task<IActionResult> GetProductById([FromQuery] GetProductByIdQueryRequest request)
        {
            var validationErrors = _productByIdValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }
        [HttpGet]
        [Route("filterItems")]
        public async Task<IActionResult> GetFilterItems([FromQuery] GetFilterItemsQueryRequest request)
        {
            var validationErrors = _filterItemsValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }
    }
}
