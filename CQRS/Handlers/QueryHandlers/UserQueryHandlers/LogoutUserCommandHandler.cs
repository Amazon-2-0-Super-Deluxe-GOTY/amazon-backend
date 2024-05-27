using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{   
    public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LogoutUserCommandHandler(DataContext dataContext, TokenService tokenService, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<string>> Handle(LogoutUserCommandRequest request, CancellationToken cancellationToken)
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
            User? user = await _dataContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);
            if (user == null)
            {
                return new("Forbidden") { statusCode = 403 };
            }
            TokenJournal? tokenJournal = await _dataContext.TokenJournals.FirstOrDefaultAsync(tj => tj.UserId == user.Id && tj.DeactivatedAt == null);
            if (tokenJournal == null)
            {
                return new("Token not found") { statusCode = 404 };
            }
            tokenJournal.DeactivatedAt = DateTime.Now;
            await _dataContext.SaveChangesAsync();
            return new() { isSuccess = true };
        }
    }
}
