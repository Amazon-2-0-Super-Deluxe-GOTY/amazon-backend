using Microsoft.AspNetCore.Mvc;
using amazon_backend.CQRS.Commands.UserRequests;
using MediatR;
using amazon_backend.Services.Response;
using FluentValidation;
using amazon_backend.Services.FluentValidation;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Authorization;
using amazon_backend.CQRS.Queries.Request.UserRequests;


namespace amazon_backend.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly RestResponseService _responseService;
        private readonly IValidator<LoginUserCommandRequest> _loginUserValidator;
        private readonly IValidator<CreateUserCommandRequest> _createUserValidator;
        private readonly IValidator<ConfirmEmailCommandRequest> _confirmEmailValidator;
        private readonly IValidator<UpdateUserCommandRequest> _updateUserlValidator;
        private readonly IValidator<UpdateUserAvatarCommandRequest> _updateUserlAvatarValidator;
        private readonly IMediator _mediator;

        public UsersController(RestResponseService responseService, IValidator<LoginUserCommandRequest> loginUserValidator, IMediator mediator, IValidator<CreateUserCommandRequest> createUserValidator, IValidator<ConfirmEmailCommandRequest> confirmEmailValidator, IValidator<UpdateUserCommandRequest> updateUserlValidator, IValidator<UpdateUserAvatarCommandRequest> updateUserlAvatarValidator)
        {
            _responseService = responseService;
            _loginUserValidator = loginUserValidator;
            _mediator = mediator;
            _createUserValidator = createUserValidator;
            _confirmEmailValidator = confirmEmailValidator;
            _updateUserlValidator = updateUserlValidator;
            _updateUserlAvatarValidator = updateUserlAvatarValidator;
        }
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            var response = await _mediator.Send(new GetUserProfileQueryRequest());
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data);
            }
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, null);
        }
        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var response = await _mediator.Send(new LogoutUserCommandRequest());
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data);
            }
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, null);
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewUser([FromBody] CreateUserCommandRequest request)
        {
            var validationErrors = _createUserValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data);
            }
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, null);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommandRequest request)
        {
            var validationErrors = _loginUserValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data);
            }
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, null);
        }

        [HttpPost("confirmEmail")]
        [Authorize]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommandRequest request)
        {
            var validationErrors = _confirmEmailValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data);
            }
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, null);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommandRequest request)
        {
            var validationErrors = _updateUserlValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data);
            }
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, null);
        }

        [HttpPut("avatar")]
        [Authorize]
        public async Task<IActionResult> UpdateUserAvatar([FromForm] UpdateUserAvatarCommandRequest request)
        {
            var validationErrors = _updateUserlAvatarValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data);
            }
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, null);
        }

        [HttpDelete("avatar")]
        [Authorize]
        public async Task<IActionResult> RemoveUserAvatar()
        {
            var response = await _mediator.Send(new RemoveUserAvatarCommandQuery());
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status200OK, "Ok", response.data);
            }
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, null);
        }

        

    }
}