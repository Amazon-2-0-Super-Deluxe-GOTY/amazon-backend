using amazon_backend.CQRS.Commands.ProductCommands;
using amazon_backend.CQRS.Queries.Request.ProductRequests;
using amazon_backend.Services.FluentValidation;
using amazon_backend.Services.Response;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IValidator<GetProductBySlugQueryRequest> _productBySlugValidator;
        private readonly IValidator<GetFilterItemsQueryRequest> _filterItemsValidator;
        private readonly IValidator<CreateProductCommandRequest> _createProductValidator;
        private readonly IValidator<UpdateProductCommandRequest> _updateProductValidator;
        private readonly IValidator<DeleteProductCommandRequest> _deleteProductValidator;

        public ProductsController(IMediator mediator, IValidator<GetProductsQueryRequest> validator, IValidator<GetProductByIdQueryRequest> productByIdValidator, RestResponseService responseService, IValidator<GetFilterItemsQueryRequest> filterItemsValidator, IValidator<CreateProductCommandRequest> createProductValidator, IValidator<UpdateProductCommandRequest> updateProductValidator, IValidator<DeleteProductCommandRequest> deleteProductValidator, IValidator<GetProductBySlugQueryRequest> productBySlugValidator)
        {
            _mediator = mediator;
            _prodByCategoryValidator = validator;
            _productByIdValidator = productByIdValidator;
            _responseService = responseService;
            _filterItemsValidator = filterItemsValidator;
            _createProductValidator = createProductValidator;
            _updateProductValidator = updateProductValidator;
            _deleteProductValidator = deleteProductValidator;
            _productBySlugValidator = productBySlugValidator;
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
        [Route("bySlug")]
        public async Task<IActionResult> GetProductBySlug([FromQuery] GetProductBySlugQueryRequest request)
        {
            var validationErrors = _productBySlugValidator.GetErrors(request);
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateNewProduct([FromBody] CreateProductCommandRequest request)
        {
            var validationErrors = _createProductValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommandRequest request)
        {
            var validationErrors = _updateProductValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteProduct([FromBody] DeleteProductCommandRequest request)
        {
            var validationErrors = _deleteProductValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }
    }
}
