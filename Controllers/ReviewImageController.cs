using amazon_backend.CQRS.Commands.ReviewImageRequests;
using amazon_backend.CQRS.Queries.Request.ReviewImageRequests;
using amazon_backend.Services.FluentValidation;
using amazon_backend.Services.Response;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [Route("api/reviewImages")]
    [ApiController]
    public class ReviewImageController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly RestResponseService _responseService;
        private readonly IValidator<CreateReviewImageCommandRequest> _createImageValidator;
        private readonly IValidator<RemoveReviewImageCommandRequest> _removeImageValidator;
        private readonly IValidator<GetReviewImageByIdQueryRequest> _getImageValidator;

        public ReviewImageController(IMediator mediator, RestResponseService responseService, IValidator<CreateReviewImageCommandRequest> createImageValidator, IValidator<RemoveReviewImageCommandRequest> removeImageValidator, IValidator<GetReviewImageByIdQueryRequest> getImageValidator)
        {
            _mediator = mediator;
            _responseService = responseService;
            _createImageValidator = createImageValidator;
            _removeImageValidator = removeImageValidator;
            _getImageValidator = getImageValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetImageById([FromQuery] GetReviewImageByIdQueryRequest request)
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
        public async Task<IActionResult> CreateNewImage([FromForm] CreateReviewImageCommandRequest request)
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
        public async Task<IActionResult> RemoveReviewImage([FromBody] RemoveReviewImageCommandRequest request)
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
