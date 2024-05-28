using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.JwtTokenProfiles;
using amazon_backend.Services.Email;
using amazon_backend.Services.JWTService;
using amazon_backend.Services.KDF;
using amazon_backend.Services.Random;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers.CommandHandlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, Result<JwtTokenProfile>>
    {
        private readonly IMediator _mediator;
        private readonly DataContext _dataContext;
        private readonly IEmailService _emailService;
        private readonly IRandomService _randomService;
        private readonly IKdfService _kdfService;

        public CreateUserCommandHandler(IMediator mediator, DataContext dataContext, IEmailService emailService, IRandomService randomService, IKdfService kdfService)
        {
            _mediator = mediator;
            _dataContext = dataContext;
            _emailService = emailService;
            _randomService = randomService;
            _kdfService = kdfService;
        }
        public async Task<Result<JwtTokenProfile>> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            User? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == request.email && u.DeletedAt != null);
            var passwordSalt = _randomService.RandomString(16);
            var emailConfirm = _randomService.ConfirmCode(6);
            if (user != null)
            {
                var passwordHash = _kdfService.GetDerivedKey(request.password, passwordSalt);
                if (passwordHash != user.PasswordHash)
                {
                    return new("Invalid email or password") { statusCode = 400 };
                }
                user.DeletedAt = null;
                await _dataContext.SaveChangesAsync();
            }
            else
            {
                User newUser = new()
                {
                    Id = Guid.NewGuid(),
                    Email = request.email,
                    PasswordHash = _kdfService.GetDerivedKey(request.password, passwordSalt),
                    PasswordSalt = passwordSalt,
                    CreatedAt = DateTime.Now,
                    Role = "User",
                    EmailCode = emailConfirm
                };

                await _emailService.SendEmailAsync(request.email, "Welcome to PERRY:)", $"Your register code: {emailConfirm}");
                await _dataContext.Users.AddAsync(newUser);
                await _dataContext.SaveChangesAsync();
            }
            var loginRequest = new LoginUserCommandRequest()
            {
                email = request.email,
                password = request.password
            };
            var result = await _mediator.Send(loginRequest);

            if (result.isSuccess)
            {
                return new(result.data) { statusCode = 201, message = "Created" };
            }

            return new(result.message) { statusCode = result.statusCode };
        }
    }
}
