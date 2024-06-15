using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers.CommandHandlers
{
    public class RemoveUsersCommandHandler : IRequestHandler<RemoveUsersCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        public RemoveUsersCommandHandler(DataContext dataContext, TokenService tokenService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
        }

        public async Task<Result<string>> Handle(RemoveUsersCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            foreach (var item in request.usersIds)
            {
                User? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(item) && u.DeletedAt == null);
                if (user != null)
                {
                    List<TokenJournal>? tJournals = await _dataContext.TokenJournals.Include(tj => tj.Token).Where(tj => tj.UserId == user.Id).ToListAsync();
                    if (tJournals != null)
                    {
                        foreach (var tJournal in tJournals)
                        {
                            if (tJournal.Token != null)
                            {
                                _dataContext.Remove(tJournal.Token);
                                await _dataContext.SaveChangesAsync();
                            }
                        }
                    }
                    user.DeletedAt = DateTime.UtcNow;
                    await _dataContext.SaveChangesAsync();
                }
            }
            return new("Ok") { statusCode = 200 };
        }
    }
}
