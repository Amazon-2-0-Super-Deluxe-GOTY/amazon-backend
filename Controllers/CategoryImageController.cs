using amazon_backend.CQRS.Commands.CategoryImageRequst;
using amazon_backend.CQRS.Commands.ReviewImageRequests;
using amazon_backend.CQRS.Queries.Request.CategoryImageRequest;
using amazon_backend.CQRS.Queries.Request.ReviewImageRequests;
using amazon_backend.Services.FluentValidation;
using amazon_backend.Services.Response;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [ApiController]
    [Route("api/categoryImages")]
    public class CategoryImageController : Controller
    {
        private readonly IMediator _mediator;
        private readonly RestResponseService _responseService;
        private readonly IValidator<CreateCategoryImageCommandRequst> _createImageValidator;
        private readonly IValidator<RemoveCategoryImageCommandRequst> _removeImageValidator;
        private readonly IValidator<GetCategoryImageByIdQueryRequst> _getImageValidator;
        public CategoryImageController(IMediator mediator, RestResponseService responseService, IValidator<CreateCategoryImageCommandRequst> createImageValidator, IValidator<RemoveCategoryImageCommandRequst> removeImageValidator, IValidator<GetCategoryImageByIdQueryRequst> getImageValidator)
        {
            _mediator = mediator;
            _responseService = responseService;
            _createImageValidator = createImageValidator;
            _removeImageValidator = removeImageValidator;
            _getImageValidator = getImageValidator;
        }
        [HttpGet]
        public async Task<IActionResult> GetImageById([FromQuery] GetCategoryImageByIdQueryRequst request)
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
        public async Task<IActionResult> CreateNewImage([FromForm] CreateCategoryImageCommandRequst request)
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
        public async Task<IActionResult> RemoveReviewImage([FromBody] RemoveCategoryImageCommandRequst request)
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
