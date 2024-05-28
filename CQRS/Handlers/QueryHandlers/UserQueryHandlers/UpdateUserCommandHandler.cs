using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.UserProfiles;
using amazon_backend.Services.JWTService;
using amazon_backend.Services.KDF;
using amazon_backend.Services.Random;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IKdfService _kdfService;
        private readonly IRandomService _randomService;

        public UpdateUserCommandHandler(DataContext dataContext, TokenService tokenService, IKdfService kdfService, IRandomService randomService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _kdfService = kdfService;
            _randomService = randomService;
        }
        public async Task<Result<string>> Handle(UpdateUserCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            if (request.birthDate == null && request.firstName == null && request.lastName == null && request.oldPassword == null && request.newPassword == null)
            {
                return new("Bad request") { data = "No parameters for update", statusCode = 400 };
            }
            if (request.birthDate != null)
            {
                user.BirthDate = DateTime.Parse(request.birthDate);
            }
            if (request.firstName != null)
            {
                user.FirstName = request.firstName;
            }
            if (request.lastName != null)
            {
                user.LastName = request.lastName;
            }
            if (request.oldPassword != null && request.newPassword != null)
            {
                var oldPasswordHash = _kdfService.GetDerivedKey(request.oldPassword, user.PasswordSalt);
                if (user.PasswordHash != oldPasswordHash)
                {
                    return new("Bad request") { data = "Invalid old password", statusCode = 400 };
                }
                var newSalt = _randomService.RandomString(16);
                user.PasswordSalt = newSalt;
                user.PasswordHash = _kdfService.GetDerivedKey(request.newPassword, newSalt);
            }
            _dataContext.Update(user);
            await _dataContext.SaveChangesAsync();
            return new() { isSuccess = true, message = "Ok", statusCode = 400 };
        }
    }
}
