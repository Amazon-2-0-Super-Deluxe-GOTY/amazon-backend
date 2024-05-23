using amazon_backend.CQRS.Commands.RewiewTagRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewTagQueryHandlers
{
    public class DeleteReviewTagCommandHandler : IRequestHandler<DeleteReviewTagCommandRequest, Result<Guid>>
    {
        private ILogger<DeleteReviewTagCommandHandler> _logger;
        private DataContext _dataContext;
        public DeleteReviewTagCommandHandler(DataContext dataContext, ILogger<DeleteReviewTagCommandHandler> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }
        public async Task<Result<Guid>> Handle(DeleteReviewTagCommandRequest request, CancellationToken cancellationToken)
        {
            if (request.reviewId != null)
            {
                try
                {
                    Review? review = await _dataContext.Reviews.Include(r => r.ReviewTags).FirstOrDefaultAsync(r => r.Id == Guid.Parse(request.reviewId));
                    if (review != null)
                    {
                        ReviewTag? rTag = await _dataContext.ReviewTags.FirstOrDefaultAsync(r => r.Id == Guid.Parse(request.reviewTagId));
                        if (rTag != null)
                        {
                            if (review.ReviewTags != null)
                            {
                                review.ReviewTags.Remove(rTag);
                                await _dataContext.SaveChangesAsync();
                                return new(rTag.Id);
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
            try
            {
                ReviewTag? reviewTag = await _dataContext.ReviewTags.Include(rt => rt.Reviews).Where(rt => rt.Id == Guid.Parse(request.reviewTagId)).FirstOrDefaultAsync();
                if (reviewTag != null)
                {
                    if (reviewTag.Reviews != null && reviewTag.Reviews.Count != 0)
                    {
                        reviewTag.Reviews.Clear();
                        await _dataContext.SaveChangesAsync(cancellationToken);
                    }
                    _dataContext.ReviewTags.Remove(reviewTag);
                    await _dataContext.SaveChangesAsync();
                    return new(reviewTag.Id);
                }
                return new("Review tag not found");
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex.Message);
                return new("See server logs") { statusCode=500};
            }
        }
    }
}
