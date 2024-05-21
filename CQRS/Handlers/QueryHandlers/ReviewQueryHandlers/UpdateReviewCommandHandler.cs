using amazon_backend.CQRS.Commands.ReviewRequests;
using amazon_backend.Data.Entity;
using amazon_backend.Data;
using amazon_backend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class UpdateReviewCommandHandler:IRequestHandler<UpdateReviewCommandRequest, Result<Guid>>
    {
        private readonly IReviewDao _reviewDao;
        private readonly DataContext _dataContext;
        public UpdateReviewCommandHandler(IReviewDao reviewDao, DataContext dataContext)
        {
            _reviewDao = reviewDao;
            _dataContext = dataContext;
        }
        public async Task<Result<Guid>> Handle(UpdateReviewCommandRequest request, CancellationToken cancellationToken)
        {
            Review? review = await _dataContext.Reviews
                .Include(r=>r.ReviewTags)
                .Where(r=>r.Id==Guid.Parse(request.reviewId)).FirstOrDefaultAsync();
            if (review != null)
            {
                if (review.ReviewTags != null && review.ReviewTags.Count != 0)
                {
                    review.ReviewTags.Clear();
                }
                review.Text = request.text;
                review.Mark = request.rating;
                if (request.reviewTagsIds != null && request.reviewTagsIds.Count != 0)
                {
                    review.ReviewTags = new List<ReviewTag>();
                    foreach (var rTag in request.reviewTagsIds)
                    {
                        ReviewTag? tag = await _dataContext.ReviewTags.Where(t => t.Id == Guid.Parse(rTag)).FirstOrDefaultAsync();
                        if (tag != null)
                        {
                            if (!review.ReviewTags.Contains(tag))
                            {
                                review.ReviewTags.Add(tag);
                            }
                        }
                    }
                }
                await _dataContext.SaveChangesAsync();
                return new(review.Id);
            }
            return new("Not found");
        }
    }
}
