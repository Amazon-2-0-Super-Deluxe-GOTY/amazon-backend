using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.Email;
using amazon_backend.Services.JWTService;
using amazon_backend.Services.Random;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers.CommandHandlers
{
    public class SendNewCodeCommandHandler : IRequestHandler<SendNewCodeCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IRandomService _randomService;

        public SendNewCodeCommandHandler(DataContext dataContext, TokenService tokenService, IEmailService emailService, IRandomService randomService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _emailService = emailService;
            _randomService = randomService;
        }

        public async Task<Result<string>> Handle(SendNewCodeCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;

            var confirmCode = _randomService.ConfirmCode(6);
            user.EmailCode = confirmCode;
            if (request.email != null)
            {
                user.Email = request.email;
            }

            _dataContext.Update(user);

            await _dataContext.SaveChangesAsync();

            var resultEmail = request.email is null ? user.Email : request.email;
            await _emailService.SendEmailAsync(resultEmail, "Welcome to PERRY:)", $"Your register code: {confirmCode}");

            return new("Ok") { statusCode = 200 };
        }
    }
}
