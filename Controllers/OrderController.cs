using amazon_backend.CQRS.Commands.OrderRequests;
using amazon_backend.CQRS.Queries.Request.OrderRequests;
using amazon_backend.Services.FluentValidation;
using amazon_backend.Services.Response;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly RestResponseService _responseService;
        private readonly IValidator<NewOrderFromCartCommandRequest> _newItemFromCartValidator;
        private readonly IValidator<GetOrdersQueryRequest> _getOrdersValidator;
        private readonly IValidator<CancelOrderCommandRequest> _cancelOrderValidator;

        public OrderController(IMediator mediator, RestResponseService responseService, IValidator<NewOrderFromCartCommandRequest> newItemFromCartValidator, IValidator<GetOrdersQueryRequest> getOrdersValidator, IValidator<CancelOrderCommandRequest> cancelOrderValidator)
        {
            _mediator = mediator;
            _responseService = responseService;
            _newItemFromCartValidator = newItemFromCartValidator;
            _getOrdersValidator = getOrdersValidator;
            _cancelOrderValidator = cancelOrderValidator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders([FromQuery] GetOrdersQueryRequest request)
        {
            var validationErrors = _getOrdersValidator.GetErrors(request);
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
        public async Task<IActionResult> NewOrderFromCart([FromBody] NewOrderFromCartCommandRequest request)
        {
            var validationErrors = _newItemFromCartValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderCommandRequest request)
        {
            var validationErrors = _cancelOrderValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

    }
}