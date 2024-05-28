using Microsoft.AspNetCore.Mvc;
using amazon_backend.CQRS.Commands.UserRequests;
using MediatR;
using amazon_backend.Services.Response;
using FluentValidation;
using amazon_backend.Services.FluentValidation;
using Microsoft.AspNetCore.Authorization;
using amazon_backend.CQRS.Queries.Request.UserRequests;
using Org.BouncyCastle.Asn1.Ocsp;


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
        private readonly IValidator<ChangeEmailCommandRequest> _changeEmailValidator;
        private readonly IMediator _mediator;

        public UsersController(RestResponseService responseService, IValidator<LoginUserCommandRequest> loginUserValidator, IMediator mediator, IValidator<CreateUserCommandRequest> createUserValidator, IValidator<ConfirmEmailCommandRequest> confirmEmailValidator, IValidator<UpdateUserCommandRequest> updateUserlValidator, IValidator<UpdateUserAvatarCommandRequest> updateUserlAvatarValidator, IValidator<ChangeEmailCommandRequest> changeEmailValidator)
        {
            _responseService = responseService;
            _loginUserValidator = loginUserValidator;
            _mediator = mediator;
            _createUserValidator = createUserValidator;
            _confirmEmailValidator = confirmEmailValidator;
            _updateUserlValidator = updateUserlValidator;
            _updateUserlAvatarValidator = updateUserlAvatarValidator;
            _changeEmailValidator = changeEmailValidator;
        }
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            var response = await _mediator.Send(new GetUserProfileQueryRequest());
            if (response.isSuccess)
            {
                return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
            }
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }
        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var response = await _mediator.Send(new LogoutUserCommandRequest());
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewUser([FromBody] CreateUserCommandRequest request)
        {
            var validationErrors = await _createUserValidator.GetErrorsAsync(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
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
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
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
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpPut("changeEmailRequest")]
        [Authorize]
        public async Task<IActionResult> SendChangeEmailRequest([FromBody] ChangeEmailCommandRequest request)
        {
            var validationErrors = await _changeEmailValidator.GetErrorsAsync(request);
            if (validationErrors != null)
            {
                return _responseService.SendResponse(HttpContext, StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await _mediator.Send(request);
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
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
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
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
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpDelete("avatar")]
        [Authorize]
        public async Task<IActionResult> RemoveUserAvatar()
        {
            var response = await _mediator.Send(new RemoveUserAvatarCommandQuery());
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveCurrentUser()
        {
            var response = await _mediator.Send(new RemoveCurrentUserCommandRequest());
            return _responseService.SendResponse(HttpContext, response.statusCode, response.message, response.data);
        }

    }
}