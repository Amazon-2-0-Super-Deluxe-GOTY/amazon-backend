using amazon_backend.CQRS.Commands.RewiewTagRequests;
using amazon_backend.CQRS.Queries.Request.ReviewsRequests;
using amazon_backend.CQRS.Queries.Request.ReviewTagRequests;
using amazon_backend.Services.FluentValidation;
using amazon_backend.Services.Response;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IValidator<DeleteReviewTagFromReviewCommandRequest> _deleteReviewTagFromReviewValidator;

        public ReviewTagsController(RestResponseService responseService, IMediator mediator, IValidator<GetReviewTagByIdQueryRequest> reviewTagByIdValidator, IValidator<CreateReviewTagCommandRequest> createReviewTagValidator, IValidator<DeleteReviewTagCommandRequest> deleteReviewTagValidator, IValidator<UpdateReviewTagCommandRequest> updateReviewTagValidator, IValidator<DeleteReviewTagFromReviewCommandRequest> deleteReviewTagFromReviewValidator)
        {
            _responseService = responseService;
            _mediator = mediator;
            _reviewTagByIdValidator = reviewTagByIdValidator;
            _createReviewTagValidator = createReviewTagValidator;
            _deleteReviewTagValidator = deleteReviewTagValidator;
            _updateReviewTagValidator = updateReviewTagValidator;
            _deleteReviewTagFromReviewValidator = deleteReviewTagFromReviewValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewTags()
        {
            var response = await _mediator.Send(new GetReviewTagsQueryRequest());
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpGet("byId")]
        public async Task<IActionResult> GetReviewTagById([FromQuery] GetReviewTagByIdQueryRequest request)
        {
            var validationErrors = _reviewTagByIdValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateNewReviewTag([FromBody] CreateReviewTagCommandRequest request)
        {
            var validationErrors = _createReviewTagValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateReviewTag([FromBody] UpdateReviewTagCommandRequest request)
        {
            var validationErrors = _updateReviewTagValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteReviewTag([FromBody] DeleteReviewTagCommandRequest request)
        {
            var validationErrors = _deleteReviewTagValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpDelete("fromReview")]
        [Authorize]
        public async Task<IActionResult> DeleteReviewTagFromReview([FromBody] DeleteReviewTagFromReviewCommandRequest request)
        {
            var validationErrors = _deleteReviewTagFromReviewValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

    }
}
