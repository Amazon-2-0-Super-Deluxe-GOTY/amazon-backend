using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.UserProfiles;
using amazon_backend.Services.Hash;
using amazon_backend.Services.JWTService;
using amazon_backend.Services.Random;
using AutoMapper;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers.CommandHandlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommandRequest, Result<ClientProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IRandomService _randomService;
        private readonly IMapper _mapper;
        private readonly IHashService<Md5HashService> _md5HashService;
        private readonly IHashService<BcryptHashService> _bcryptHashService;

        public UpdateUserCommandHandler(DataContext dataContext, TokenService tokenService, IRandomService randomService, IMapper mapper, IHashService<Md5HashService> md5HashService, IHashService<BcryptHashService> bcryptHashService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _randomService = randomService;
            _mapper = mapper;
            _md5HashService = md5HashService;
            _bcryptHashService = bcryptHashService;
        }
        public async Task<Result<ClientProfile>> Handle(UpdateUserCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            if (request.birthDate == null && request.firstName == null && request.lastName == null && request.oldPassword == null && request.newPassword == null)
            {
                return new("No parameters for update") { statusCode = 400 };
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
                var result = _md5HashService.VerifyPassword(request.oldPassword, user.PasswordSalt, user.PasswordHash);
                if (!result)
                {
                    result = _bcryptHashService.VerifyPassword(request.oldPassword, user.PasswordSalt, user.PasswordHash);
                    if (result == false) return new("Invalid old password") { statusCode = 400 };
                }
                var newSalt = _randomService.RandomString(16);
                user.PasswordSalt = newSalt;
                user.PasswordHash = _bcryptHashService.HashPassword(request.newPassword, newSalt);
            }
            _dataContext.Update(user);
            await _dataContext.SaveChangesAsync();
            return new(_mapper.Map<ClientProfile>(user)) { isSuccess = true, message = "Ok", statusCode = 200 };
        }
    }
}
