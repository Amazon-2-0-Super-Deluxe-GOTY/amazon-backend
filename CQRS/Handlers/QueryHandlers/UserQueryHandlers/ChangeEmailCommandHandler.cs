using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.Email;
using amazon_backend.Services.JWTService;
using amazon_backend.Services.Random;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{
    public class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IRandomService _randomService;

        public ChangeEmailCommandHandler(DataContext dataContext, TokenService tokenService, IEmailService emailService, IRandomService randomService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _emailService = emailService;
            _randomService = randomService;
        }
        public async Task<Result<string>> Handle(ChangeEmailCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            user.TempEmail = request.newEmail;
            user.EmailCode = _randomService.ConfirmCode(6);
            _dataContext.Update(user);
            await _dataContext.SaveChangesAsync();

            await _emailService.SendEmailAsync(request.newEmail, "Perry! Change email request", $"Your code:{user.EmailCode}");

            return new("Ok") { statusCode = 200 };
        }
    }
}
