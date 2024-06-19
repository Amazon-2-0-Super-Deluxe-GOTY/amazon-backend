using amazon_backend.CQRS.Queries.Request.UserRequests;
using amazon_backend.Data;
using amazon_backend.Models;
using amazon_backend.Profiles.UserProfiles;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers.QueryHandlers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQueryRequest, Result<List<ClientProfile>>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IMapper mapper, TokenService tokenService, DataContext dataContext)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _dataContext = dataContext;
        }

        public async Task<Result<List<ClientProfile>>> Handle(GetUsersQueryRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }

            var userQuery = _dataContext.Users.AsQueryable();
            if (!string.IsNullOrEmpty(request.searchQuery))
            {
                userQuery = userQuery.Where(u => (u.FirstName + " " + u.LastName).Contains(request.searchQuery));
            }
            if (!string.IsNullOrEmpty(request.role))
            {
                switch (request.role.ToLower())
                {
                    case "admin":
                        userQuery = userQuery.Where(u => u.Role == "Admin");
                        break;
                    case "user":
                        userQuery = userQuery.Where(u => u.Role == "User");
                        break;
                }
            }
            if (!string.IsNullOrEmpty(request.orderBy))
            {
                if (request.orderBy.ToLower() == "email")
                {
                    userQuery = userQuery.OrderBy(u => u.Email);
                }
                else
                {
                    userQuery = userQuery.OrderBy(u => u.CreatedAt);
                }
            }
            else
            {
                userQuery = userQuery.OrderBy(u => u.CreatedAt);
            }

            var users = await userQuery
                .Skip(request.pageSize * (request.pageIndex - 1))
                .Take(request.pageSize)
                .ToListAsync();

            if (users != null && users.Count != 0)
            {
                var productProfiles = _mapper.Map<List<ClientProfile>>(users);
                int totalCount = await userQuery.CountAsync();
                int pagesCount = (int)Math.Ceiling(totalCount / (double)request.pageSize);
                return new(productProfiles, pagesCount) { statusCode = 200 };
            }
            return new("Users not found") { statusCode = 404 };
        }
    }
}
