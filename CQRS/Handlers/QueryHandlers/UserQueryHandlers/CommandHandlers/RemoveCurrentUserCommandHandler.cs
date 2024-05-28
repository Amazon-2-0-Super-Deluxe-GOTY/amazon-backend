using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers.CommandHandlers
{
    public class RemoveCurrentUserCommandHandler : IRequestHandler<RemoveCurrentUserCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        public RemoveCurrentUserCommandHandler(DataContext dataContext, TokenService tokenService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
        }
        public async Task<Result<string>> Handle(RemoveCurrentUserCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            List<TokenJournal>? tJournals = await _dataContext.TokenJournals.Include(tj=>tj.Token).Where(tj => tj.UserId == user.Id).ToListAsync();
            if (tJournals != null)
            {
                foreach(var tJournal in tJournals)
                {
                    if (tJournal.Token != null)
                    {
                        _dataContext.Remove(tJournal.Token);
                        await _dataContext.SaveChangesAsync();
                    }
                }
            }
            user.DeletedAt= DateTime.UtcNow;
            _dataContext.Update(user);
            await _dataContext.SaveChangesAsync();
            return new() { message = "Ok", statusCode = 200 };
        }
    }
}
