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
        public ConfirmEmailCommandHandler(TokenService tokenService, DataContext dataContext)
        {
            _tokenService = tokenService;
            _dataContext = dataContext;
        }

        public async Task<Result<string>> Handle(ConfirmEmailCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            if (user.EmailCode == null)
            {
                return new("Bad request") { data = "Email already confirmed", statusCode = 400 };
            }
            if (user.EmailCode == request.emailCode)
            {
                if (user.TempEmail != null)
                {
                    user.EmailCode = null;
                    user.Email = user.TempEmail;
                    user.TempEmail = null;
                    _dataContext.Update(user);
                    await _dataContext.SaveChangesAsync();
                    return new() { message = "Ok", data = "Email successfully updated", statusCode = 200 };
                }
                user.EmailCode = null;
                _dataContext.Update(user);
                await _dataContext.SaveChangesAsync();
                return new() { message = "Ok", data = "Email successfully confirmed", statusCode = 200 };
            }
            else
            {
                return new("Bad Request") { message = "Invalid email code", statusCode = 400 };
            }
        }
    }
}
