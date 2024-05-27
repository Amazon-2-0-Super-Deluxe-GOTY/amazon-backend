using amazon_backend.CQRS.Queries.Request.UserRequests;
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
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQueryRequest, Result<ClientProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetUserProfileQueryHandler(IMapper mapper, TokenService tokenService, IHttpContextAccessor httpContextAccessor, DataContext dataContext)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _dataContext = dataContext;
        }
        public async Task<Result<ClientProfile>> Handle(GetUserProfileQueryRequest request, CancellationToken cancellationToken)
        {
            if(_httpContextAccessor.HttpContext == null)
            {
                return new("Token rejected") { statusCode = 401 };
            }
            Guid? userId = await _tokenService.DecodeTokenFromHeaders(_httpContextAccessor.HttpContext);
            if(userId == null)
            {
                return new("Token rejected") { statusCode = 401 };
            }
            User? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);
            if (user != null)
            {
                return new(_mapper.Map<ClientProfile>(user));
            }
            return new("Forbidden") { statusCode = 403 };
        }
    }
}
