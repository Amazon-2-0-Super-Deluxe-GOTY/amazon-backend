using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.JwtTokenProfiles;
using amazon_backend.Services.Hash;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers.CommandHandlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, Result<JwtTokenProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IHashService<Md5HashService> _md5HashService;
        private readonly IHashService<BcryptHashService> _bcryptHashService;

        public LoginUserCommandHandler(DataContext dataContext, TokenService tokenService, IHttpContextAccessor httpContext, IMapper mapper, IHashService<BcryptHashService> bcryptHashService, IHashService<Md5HashService> md5HashService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _httpContextAccessor = httpContext;
            _mapper = mapper;
            _bcryptHashService = bcryptHashService;
            _md5HashService = md5HashService;
        }

        public async Task<Result<JwtTokenProfile>> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            User? user = await _dataContext.Users
                .FirstOrDefaultAsync(u => u.Email == request.email && u.DeletedAt == null);
            if (user == null)
            {
                return new("Invalid email or password") { statusCode = 404 };
            }

            if (_md5HashService.VerifyPassword(request.password, user.PasswordSalt, user.PasswordHash))
            {
                user.PasswordHash = _bcryptHashService.HashPassword(request.password);
                await _dataContext.SaveChangesAsync();
            }

            else if(!_bcryptHashService.VerifyPassword(request.password,user.PasswordSalt,user.PasswordHash))
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
                    Expires = token.ExpirationDate.ToUniversalTime()
                };
                httpContext.Response.Cookies.Append("jwt", token.Token, cookieOptions);
            }
            return new(_mapper.Map<JwtTokenProfile>(token)) { message = "Ok", statusCode = 200 };
        }
    }
}
