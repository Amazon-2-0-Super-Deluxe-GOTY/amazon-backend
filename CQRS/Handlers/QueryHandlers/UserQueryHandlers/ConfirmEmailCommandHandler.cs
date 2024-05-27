using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.UserProfiles;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ConfirmEmailCommandHandler(IHttpContextAccessor httpContextAccessor, TokenService tokenService, DataContext dataContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
            _dataContext = dataContext;
        }

        public async Task<Result<string>> Handle(ConfirmEmailCommandRequest request, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return new("Token rejected") { statusCode = 401 };
            }
            Guid? userId = await _tokenService.DecodeTokenFromHeaders(_httpContextAccessor.HttpContext);
            if (userId == null)
            {
                return new("Token rejected") { statusCode = 401 };
            }
            User? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);
            if (user != null)
            {
                if (user.EmailCode == null)
                {
                    return new("Email already confirmed") { statusCode = 400 };
                }
                if (user.EmailCode == request.emailCode)
                {
                    user.EmailCode = null;
                    await _dataContext.SaveChangesAsync();
                    return new() { isSuccess = true, data = "Email successfully confirmed" };
                }
                else
                {
                    return new("Invalid email code") { statusCode = 400 };
                }
            }
            return new("Forbidden") { statusCode = 403 };
        }
    }
}
