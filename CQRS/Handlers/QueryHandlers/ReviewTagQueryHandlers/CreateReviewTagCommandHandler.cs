using amazon_backend.CQRS.Commands.RewiewTagRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewTagQueryHandlers
{
    public class CreateReviewTagCommandHandler : IRequestHandler<CreateReviewTagCommandRequest, Result<ReviewTagProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<CreateReviewTagCommandHandler> _logger;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;

        public CreateReviewTagCommandHandler(DataContext dataContext, ILogger<CreateReviewTagCommandHandler> logger, TokenService tokenService, IMapper mapper)
        {
            _dataContext = dataContext;
            _logger = logger;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        public async Task<Result<ReviewTagProfile>> Handle(CreateReviewTagCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            if (user.Role == "Admin")
            {
                ReviewTag newTag = new()
                {
                    Id = Guid.NewGuid(),
                    Name = request.name,
                    Description = request.description,
                    CreatedAt = DateTime.Now,
                };
                try
                {
                    await _dataContext.ReviewTags.AddAsync(newTag, cancellationToken);
                    await _dataContext.SaveChangesAsync(cancellationToken);
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError(ex.Message);
                    return new("See server logs") { statusCode = 500 };
                }
                return new(_mapper.Map<ReviewTagProfile>(newTag)) { message = "Created", statusCode = 201 };
            }
            return new("Forbidden") { statusCode = 403 };
        }
    }
}
