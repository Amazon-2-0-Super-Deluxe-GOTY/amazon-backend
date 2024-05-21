using amazon_backend.CQRS.Commands.RewiewTagRequests;
using amazon_backend.CQRS.Queries.Request.ReviewsRequests;
using amazon_backend.CQRS.Queries.Request.ReviewTagRequests;
using amazon_backend.Services.FluentValidation;
using amazon_backend.Services.Response;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [Route("api/reviewTags")]
    [ApiController]
    public class ReviewTagsController : ControllerBase
    {
        private readonly RestResponseService _responseService;
        private readonly IMediator _mediator;
        private readonly IValidator<GetReviewTagByIdQueryRequest> _reviewTagByIdValidator;
        private readonly IValidator<CreateReviewTagCommandRequest> _createReviewTagValidator;
        private readonly IValidator<DeleteReviewTagCommandRequest> _deleteReviewTagValidator;
        private readonly IValidator<UpdateReviewTagCommandRequest> _updateReviewTagValidator;
        public ReviewTagsController(RestResponseService responseService, IMediator mediator, IValidator<GetReviewTagByIdQueryRequest> reviewTagByIdValidator, IValidator<CreateReviewTagCommandRequest> createReviewTagValidator, IValidator<DeleteReviewTagCommandRequest> deleteReviewTagValidator, IValidator<UpdateReviewTagCommandRequest> updateReviewTagValidator)
        {
            _responseService = responseService;
            _mediator = mediator;
            _reviewTagByIdValidator = reviewTagByIdValidator;
            _createReviewTagValidator = createReviewTagValidator;
            _deleteReviewTagValidator = deleteReviewTagValidator;
            _updateReviewTagValidator = updateReviewTagValidator;
        }
        [HttpGet]
        public async Task<IActionResult> GetReviewTags([FromQuery] GetReviewTagsQueryRequest request)
        {
            var response = await _mediator.Send(request);
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data);
            }
            return _responseService.SendResponse(HttpContext, StatusCodes.Status404NotFound, response.message, null);
        }
        [HttpGet("ById")]
        public async Task<IActionResult> GetReviewTagById([FromQuery] GetReviewTagByIdQueryRequest request)
        {
            var validationErrors = _reviewTagByIdValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data);
            }
            return _responseService.SendResponse(HttpContext, StatusCodes.Status404NotFound, response.message, null);
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewReviewTag([FromForm] CreateReviewTagCommandRequest request)
        {
            var validationErrors = _createReviewTagValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data);
            }
            return _responseService.SendResponse(HttpContext, StatusCodes.Status500InternalServerError, response.message, null);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteReviewTag([FromQuery] DeleteReviewTagCommandRequest request)
        {
            var validationErrors = _deleteReviewTagValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok",null);
            }
            return _responseService.SendResponse(HttpContext, StatusCodes.Status404NotFound, response.message, null);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateReviewTag([FromForm] UpdateReviewTagCommandRequest request)
        {
            var validationErrors = _updateReviewTagValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data);
            }
            return _responseService.SendResponse(HttpContext, StatusCodes.Status404NotFound, response.message, null);
        }
    }
}
