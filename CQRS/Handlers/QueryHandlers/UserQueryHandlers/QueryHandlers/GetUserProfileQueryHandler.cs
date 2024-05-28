using amazon_backend.CQRS.Queries.Request.UserRequests;
using amazon_backend.Models;
using amazon_backend.Profiles.UserProfiles;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.UserQueryHandlers.QueryHandlers
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQueryRequest, Result<ClientProfile>>
    {
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;
        public GetUserProfileQueryHandler(IMapper mapper, TokenService tokenService)
        {
            _mapper = mapper;
            _tokenService = tokenService;
        }
        public async Task<Result<ClientProfile>> Handle(GetUserProfileQueryRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            return new(_mapper.Map<ClientProfile>(decodeResult.data)) { message = "Ok", statusCode = 200 };
        }
    }
}
