using amazon_backend.CQRS.Commands.ReviewImageRequests;
using amazon_backend.CQRS.Queries.Request.ProductImageRequests;
using amazon_backend.Services.FluentValidation;
using amazon_backend.Services.Response;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [ApiController]
    [Route("api/productImages")]
    public class ProductImagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly RestResponseService _responseService;
        private readonly IValidator<CreateProductImageCommandRequest> _createImageValidator;
        private readonly IValidator<GetProductImageByIdQueryRequest> _getImageValidator;
        private readonly IValidator<RemoveProductImageCommandRequest> _removeImageValidator;

        public ProductImagesController(IMediator mediator, RestResponseService responseService, IValidator<CreateProductImageCommandRequest> createImageValidator, IValidator<GetProductImageByIdQueryRequest> getImageValidator, IValidator<RemoveProductImageCommandRequest> removeImageValidator)
        {
            _mediator = mediator;
            _responseService = responseService;
            _createImageValidator = createImageValidator;
            _getImageValidator = getImageValidator;
            _removeImageValidator = removeImageValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetImageById([FromQuery] GetProductImageByIdQueryRequest request)
        {
            var validationErrors = _getImageValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateNewImage([FromForm] CreateProductImageCommandRequest request)
        {
            var validationErrors = _createImageValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveProductImage([FromBody] RemoveProductImageCommandRequest request)
        {
            var validationErrors = _removeImageValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }
    }
}
