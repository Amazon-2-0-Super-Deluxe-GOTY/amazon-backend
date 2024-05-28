using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.JwtTokenProfiles;
using amazon_backend.Services.JWTService;
using amazon_backend.Services.KDF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, Result<JwtTokenProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IKdfService _kdfService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginUserCommandHandler(DataContext dataContext, TokenService tokenService, IKdfService kdfService, IHttpContextAccessor httpContext)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _kdfService = kdfService;
            _httpContextAccessor = httpContext;
        }

        public async Task<Result<JwtTokenProfile>> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            User? user = await _dataContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == request.email && u.DeletedAt == null);
            if (user == null)
            {
                return new("Invalid email or password") { statusCode = 404 };
            }
            var passwordHash = _kdfService.GetDerivedKey(request.password, user.PasswordSalt);
            if (user.PasswordHash != passwordHash)
            {
                return new("Invalid email or password") { statusCode = 404 };
            }
            var token = await _tokenService.GetTokenByUserId(user.Id);
            if (token == null)
            {
                return new("Auth rejected") { statusCode = 401 };
            }
            return new(token) { message = "Ok", statusCode = 200 };
        }
    }
}
