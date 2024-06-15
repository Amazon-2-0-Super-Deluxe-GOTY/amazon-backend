using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.UserProfiles;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers.CommandHandlers
{
    public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommandRequest, Result<ClientProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;

        public ChangeUserRoleCommandHandler(DataContext dataContext, TokenService tokenService, IMapper mapper)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<Result<ClientProfile>> Handle(ChangeUserRoleCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(request.userId));
            if (user == null)
            {
                return new("User not found") { statusCode = 404 };
            }
            user.Role = user.Role == "Admin" ? "User" : "Admin";
            await _dataContext.SaveChangesAsync();
            return new("Ok") { statusCode = 200, data = _mapper.Map<ClientProfile>(user) };
        }
    }
}
