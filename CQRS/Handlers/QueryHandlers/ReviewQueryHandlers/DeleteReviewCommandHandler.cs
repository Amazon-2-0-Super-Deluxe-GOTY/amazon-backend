using amazon_backend.CQRS.Commands.ReviewRequests;
using amazon_backend.Models;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommandRequest, Result<Guid>>
    {
        private readonly IReviewDao _reviewDao;
        public DeleteReviewCommandHandler(IReviewDao reviewDao)
        {
            _reviewDao = reviewDao;
        }
        public async Task<Result<Guid>> Handle(DeleteReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var reviewId = Guid.Parse(request.reviewId);
            var result = await _reviewDao.DeleteAsync(reviewId);
            if (result)
            {
                return new(reviewId);
            }
            else return new("Review not found");
        }
    }
}
