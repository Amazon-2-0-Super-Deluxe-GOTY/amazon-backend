using amazon_backend.CQRS.Commands.RewiewTagRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewTagQueryHandlers
{
    public class DeleteReviewTagFromReviewCommandHandler : IRequestHandler<DeleteReviewTagFromReviewCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly ILogger<DeleteReviewTagFromReviewCommandHandler> _logger;

        public DeleteReviewTagFromReviewCommandHandler(DataContext dataContext, TokenService tokenService, ILogger<DeleteReviewTagFromReviewCommandHandler> logger)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(DeleteReviewTagFromReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            try
            {
                Review? review = await _dataContext.Reviews.Include(r => r.ReviewTags).FirstOrDefaultAsync(r => r.Id == Guid.Parse(request.reviewId));
                if (review != null && review.UserId == user.Id) 
                {
                    ReviewTag? rTag = await _dataContext.ReviewTags.FirstOrDefaultAsync(r => r.Id == Guid.Parse(request.reviewTagId));
                    if (rTag != null)
                    {
                        if (review.ReviewTags != null)
                        {
                            review.ReviewTags.Remove(rTag);
                            await _dataContext.SaveChangesAsync();
                            return new("Ok") { statusCode = 200 };
                        }
                    }
                    return new("Review tag not found") { statusCode = 404 };
                }
                return new("Review not found") { statusCode = 404 };
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex.Message);
                return new("See server logs") { statusCode = 500 };
            }
        }
    }
}
