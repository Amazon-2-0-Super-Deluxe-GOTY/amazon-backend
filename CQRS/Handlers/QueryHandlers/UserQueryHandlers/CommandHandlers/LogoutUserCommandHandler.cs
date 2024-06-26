﻿using amazon_backend.CQRS.Commands.UserRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Migrations;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LogoutUserCommandHandler(DataContext dataContext, TokenService tokenService, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
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
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                httpContext.Response.Cookies.Append("jwt", string.Empty, cookieOptions);
            }
            return new() { message = "Ok", statusCode = 200 };
        }
    }
}
