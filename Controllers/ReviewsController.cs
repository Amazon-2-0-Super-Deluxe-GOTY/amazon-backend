using amazon_backend.CQRS.Commands.ReviewRequests;
using amazon_backend.CQRS.Queries.Request.ReviewsRequests;
using amazon_backend.Services.FluentValidation;
using amazon_backend.Services.Response;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly RestResponseService _responseService;
        private readonly IValidator<GetReviewByIdQueryRequest> _reviewByIdValidator;
        private readonly IValidator<GetReviewsQueryRequest> _reviewsValidator;
        private readonly IValidator<CreateReviewCommandRequest> _reviewCreateValidator;
        private readonly IValidator<DeleteReviewCommandRequest> _reviewDeleteValidator;
        private readonly IValidator<UpdateReviewCommandRequest> _reviewUpdateValidator;

        public ReviewsController(IMediator mediator, RestResponseService responseService, IValidator<GetReviewByIdQueryRequest> reviewByIdValidator, IValidator<GetReviewsQueryRequest> reviewsValidator, IValidator<CreateReviewCommandRequest> reviewCreateValidator, IValidator<DeleteReviewCommandRequest> reviewDeleteValidator, IValidator<UpdateReviewCommandRequest> reviewUpdateValidator)
        {
            _mediator = mediator;
            _responseService = responseService;
            _reviewByIdValidator = reviewByIdValidator;
            _reviewsValidator = reviewsValidator;
            _reviewCreateValidator = reviewCreateValidator;
            _reviewDeleteValidator = reviewDeleteValidator;
            _reviewUpdateValidator = reviewUpdateValidator;
        }
        [HttpGet]
        public async Task<IActionResult> GetReviews([FromQuery] GetReviewsQueryRequest request)
        {
            var validationErrors = _reviewsValidator.GetErrors(request);
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

        [HttpGet("byId")]
        public async Task<IActionResult> GetReviewById([FromQuery] GetReviewByIdQueryRequest request)
        {
            var validationErrors = _reviewByIdValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpPost("newReview")]
        [Authorize]
        public async Task<IActionResult> CreateNewReview([FromBody] CreateReviewCommandRequest request)
        {
            var validationErrors = _reviewCreateValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewCommandRequest request)
        {
            var validationErrors = _reviewUpdateValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteReview([FromBody] DeleteReviewCommandRequest request)
        {
            var validationErrors = _reviewDeleteValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

    }
}
