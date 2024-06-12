using amazon_backend.CQRS.Commands.WishListRequests;
using amazon_backend.CQRS.Queries.Request.WishListRequests;
using amazon_backend.Services.FluentValidation;
using amazon_backend.Services.Response;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    public class WishListController : ControllerBase
    {

        private readonly RestResponseService _responseService;
        private readonly IMediator _mediator;
        private readonly IValidator<AddItemToWishCommandRequest> _addItemValidator;
        private readonly IValidator<RemoveItemsFromWishCommandRequest> _removeItemsValidator;
        private readonly IValidator<GetWishListQueryRequest> _getItemsValidator;

        public WishListController(RestResponseService responseService, IMediator mediator, IValidator<AddItemToWishCommandRequest> addItemValidator, IValidator<RemoveItemsFromWishCommandRequest> removeItemsValidator, IValidator<GetWishListQueryRequest> getItemsValidator)
        {
            _responseService = responseService;
            _mediator = mediator;
            _addItemValidator = addItemValidator;
            _removeItemsValidator = removeItemsValidator;
            _getItemsValidator = getItemsValidator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetItemFromWishList([FromQuery] GetWishListQueryRequest request)
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
        public async Task<IActionResult> AddNewItemToWishList([FromBody]AddItemToWishCommandRequest request)
        {
            var validationErrors = _addItemValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveItemsFromWishList([FromBody] RemoveItemsFromWishCommandRequest request)
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