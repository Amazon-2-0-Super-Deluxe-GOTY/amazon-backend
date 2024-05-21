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
                return new("See server logs");
            }
        }
    }
}
