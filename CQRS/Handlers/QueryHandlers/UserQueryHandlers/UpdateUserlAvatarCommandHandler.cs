using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data.Entity;
using amazon_backend.Data;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using MediatR;
using amazon_backend.Services.AWSS3;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{
    public class UpdateUserlAvatarCommandHandler : IRequestHandler<UpdateUserAvatarCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IS3Service _s3Service;
        public UpdateUserlAvatarCommandHandler(DataContext dataContext, TokenService tokenService, IS3Service s3Service)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _s3Service = s3Service;
        }
        public async Task<Result<string>> Handle(UpdateUserAvatarCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            if (user.AvatarUrl != null)
            {
                await _s3Service.DeleteFile(user.AvatarUrl);
            }
            string? newAvatar = await _s3Service.UploadFile(request.userAvatar, "users");
            if (newAvatar != null)
            {
                user.AvatarUrl = newAvatar;
                _dataContext.Update(user);
                await _dataContext.SaveChangesAsync();
                return new() { message = "Ok", statusCode = 200 };
            }
            return new("See server logs") { statusCode = 500 };
        }
    }
}
