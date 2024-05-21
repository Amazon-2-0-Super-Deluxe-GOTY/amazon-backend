using amazon_backend.CQRS.Commands.ReviewRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommandRequest, Result<Guid>>
    {
        private readonly IReviewDao _reviewDao;
        private readonly DataContext _dataContext;
        public CreateReviewCommandHandler(IReviewDao reviewDao, DataContext dataContext)
        {
            _reviewDao = reviewDao;
            _dataContext = dataContext;
        }
        public async Task<Result<Guid>> Handle(CreateReviewCommandRequest request, CancellationToken cancellationToken)
        {
            Review newReview = new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(request.userId),
                ProductId = Guid.Parse(request.productId),
                Text = string.IsNullOrEmpty(request.text) ? null : request.text,
                CreatedAt = DateTime.Now,
                Mark = request.rating,
            };
            //s3 logic
            if (request.reviewTagsIds != null && request.reviewTagsIds.Count != 0)
            {
                newReview.ReviewTags = new List<ReviewTag>();
                foreach (var rTag in request.reviewTagsIds)
                {
                    ReviewTag? tag = await _dataContext.ReviewTags.Where(t => t.Id == Guid.Parse(rTag)).FirstOrDefaultAsync();
                    if (tag != null)
                    {
                        if (!newReview.ReviewTags.Contains(tag))
                        {
                            newReview.ReviewTags.Add(tag);
                        }
                    }
                }
            }
            var result = await _reviewDao.AddAsync(newReview);
            if (result)
            {
                return new(newReview.Id);
            }
            return new("See server logs");
        }
    }
}
