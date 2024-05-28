using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.AWSS3;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers.CommandHandlers
{
    public class RemoveUserAvatarCommandHandler : IRequestHandler<RemoveUserAvatarCommandQuery, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IS3Service _s3Service;
        public RemoveUserAvatarCommandHandler(DataContext dataContext, TokenService tokenService, IS3Service s3Service)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _s3Service = s3Service;
        }
        public async Task<Result<string>> Handle(RemoveUserAvatarCommandQuery request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            if (user.AvatarUrl != null)
            {
                var result = await _s3Service.DeleteFile(user.AvatarUrl);
                if (result)
                {
                    user.AvatarUrl = null;
                    _dataContext.Update(user);
                    await _dataContext.SaveChangesAsync();
                    return new() { message = "Ok", statusCode = 200 };
                }
            }
            return new("Bad request") { data = "Already deleted", statusCode = 400 };
        }
    }
}
