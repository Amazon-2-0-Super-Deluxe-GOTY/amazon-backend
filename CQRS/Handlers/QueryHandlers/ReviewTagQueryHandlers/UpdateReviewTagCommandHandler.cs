using amazon_backend.CQRS.Commands.RewiewTagRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewTagQueryHandlers
{
    public class UpdateReviewTagCommandHandler : IRequestHandler<UpdateReviewTagCommandRequest, Result<ReviewTagProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<UpdateReviewTagCommandHandler> _logger;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;

        public UpdateReviewTagCommandHandler(DataContext dataContext, ILogger<UpdateReviewTagCommandHandler> logger, TokenService tokenService, IMapper mapper)
        {
            _dataContext = dataContext;
            _logger = logger;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        public async Task<Result<ReviewTagProfile>> Handle(UpdateReviewTagCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            if (user.Role == "Admin")
            {
                try
                {
                    ReviewTag? reviewTag = await _dataContext.ReviewTags.Where(rt => rt.Id == Guid.Parse(request.reviewTagId)).FirstOrDefaultAsync();
                    if (reviewTag != null)
                    {
                        reviewTag.Name = request.name;
                        reviewTag.Description = request.description;
                        await _dataContext.SaveChangesAsync();
                        return new("Ok") { statusCode = 200, data = _mapper.Map<ReviewTagProfile>(reviewTag) };
                    }
                    return new("Not Found") { statusCode = 404 };
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError(ex.Message);
                    return new("See server logs") { statusCode = 500 };
                }
            }
            return new("Forbidden") { statusCode = 403 };
        }
    }
}
