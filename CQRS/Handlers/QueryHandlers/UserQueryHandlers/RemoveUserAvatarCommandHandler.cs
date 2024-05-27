using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.AWSS3;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{
    public class RemoveUserAvatarCommandHandler : IRequestHandler<RemoveUserAvatarCommandQuery, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TokenService _tokenService;
        private readonly IS3Service _s3Service;
        public RemoveUserAvatarCommandHandler(DataContext dataContext, IHttpContextAccessor httpContextAccessor, TokenService tokenService, IS3Service s3Service)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
            _s3Service = s3Service;
        }
        public async Task<Result<string>> Handle(RemoveUserAvatarCommandQuery request, CancellationToken cancellationToken)
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
            if (user == null)
            {
                return new("Forbidden") { statusCode = 403 };
            }
            if (user.AvatarUrl != null)
            {
                var result = await _s3Service.DeleteFile(user.AvatarUrl);
                if (result)
                {
                    user.AvatarUrl = null;
                    await _dataContext.SaveChangesAsync();
                    return new() { isSuccess = true };
                }
            }
            return new("Already deleted") { statusCode = 400 };
        }
    }
}
