using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.JwtTokenProfiles;
using amazon_backend.Services.JWTService;
using amazon_backend.Services.KDF;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers.CommandHandlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, Result<JwtTokenProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IKdfService _kdfService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public LoginUserCommandHandler(DataContext dataContext, TokenService tokenService, IKdfService kdfService, IHttpContextAccessor httpContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _kdfService = kdfService;
            _httpContextAccessor = httpContext;
            _mapper = mapper;
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
            var token = await _tokenService.GetTokenByUserId(user.Id, request.staySignedIn);
            if (token == null)
            {
                return new("Auth rejected") { statusCode = 401 };
            }
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = token.ExpirationDate
                };
                httpContext.Response.Cookies.Append("jwt", token.Token, cookieOptions);
            }
            return new(_mapper.Map<JwtTokenProfile>(token)) { message = "Ok", statusCode = 200 };
        }
    }
}
