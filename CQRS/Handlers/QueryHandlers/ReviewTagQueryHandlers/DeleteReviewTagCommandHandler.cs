using amazon_backend.CQRS.Commands.RewiewTagRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewTagQueryHandlers
{
    public class DeleteReviewTagCommandHandler : IRequestHandler<DeleteReviewTagCommandRequest, Result<string>>
    {
        private readonly ILogger<DeleteReviewTagCommandHandler> _logger;
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;

        public DeleteReviewTagCommandHandler(DataContext dataContext, ILogger<DeleteReviewTagCommandHandler> logger, TokenService tokenService)
        {
            _dataContext = dataContext;
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task<Result<string>> Handle(DeleteReviewTagCommandRequest request, CancellationToken cancellationToken)
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
                    ReviewTag? reviewTag = await _dataContext.ReviewTags
                        .Include(rt => rt.Reviews)
                        .FirstOrDefaultAsync(rt => rt.Id == Guid.Parse(request.reviewTagId));
                    if (reviewTag != null)
                    {
                        if (reviewTag.Reviews != null && reviewTag.Reviews.Count != 0)
                        {
                            reviewTag.Reviews.Clear();
                        }
                        _dataContext.ReviewTags.Remove(reviewTag);
                        await _dataContext.SaveChangesAsync();
                        return new("Ok") { statusCode = 200 };
                    }
                    return new("Review tag not found") { statusCode = 404 };
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
