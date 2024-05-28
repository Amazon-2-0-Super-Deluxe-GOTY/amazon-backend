using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers.CommandHandlers
{
    public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        public LogoutUserCommandHandler(DataContext dataContext, TokenService tokenService, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
        }
        public async Task<Result<string>> Handle(LogoutUserCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            TokenJournal? tokenJournal = await _dataContext.TokenJournals.FirstOrDefaultAsync(tj => tj.UserId == user.Id && tj.DeactivatedAt == null);
            if (tokenJournal == null)
            {
                return new("Bad request") { data = "Token not found", statusCode = 404 };
            }
            tokenJournal.DeactivatedAt = DateTime.Now;
            await _dataContext.SaveChangesAsync();
            return new() { message = "Ok", statusCode = 200 };
        }
    }
}
