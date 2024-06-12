using amazon_backend.CQRS.Commands.CartRequests;
using amazon_backend.CQRS.Queries.Request.CartRequests;
using amazon_backend.Services.FluentValidation;
using amazon_backend.Services.Response;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{

    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly RestResponseService _responseService;
        private readonly IValidator<AddNewItemToCartCommandRequest> _addNewItemValidator;
        private readonly IValidator<GetItemsFromCartQueryRequest> _getItemsValidator;
        private readonly IValidator<RemoveItemsFromCartCommandRequest> _removeItemsValidator;
        private readonly IValidator<UpdateCartItemCommandRequest> _updateItemValidator;

        public CartController(IMediator mediator, RestResponseService responseService, IValidator<AddNewItemToCartCommandRequest> addNewItemValidator, IValidator<GetItemsFromCartQueryRequest> getItemsValidator, IValidator<RemoveItemsFromCartCommandRequest> removeItemsValidator, IValidator<UpdateCartItemCommandRequest> updateItemValidator)
        {
            _mediator = mediator;
            _responseService = responseService;
            _addNewItemValidator = addNewItemValidator;
            _getItemsValidator = getItemsValidator;
            _removeItemsValidator = removeItemsValidator;
            _updateItemValidator = updateItemValidator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCart([FromQuery] GetItemsFromCartQueryRequest request)
        {
            var validationErrors = _getItemsValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data,
                    new() { currentPage = request.pageIndex, pagesCount = response.pagesCount });
            }
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddNewItemToCart([FromBody] AddNewItemToCartCommandRequest request)
        {
            var validationErrors = _addNewItemValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemCommandRequest request)
        {
            var validationErrors = _updateItemValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteItemsFromCart([FromBody] RemoveItemsFromCartCommandRequest request)
        {
            var validationErrors = _removeItemsValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

    }
}
