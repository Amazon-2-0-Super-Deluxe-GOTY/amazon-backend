using amazon_backend.CQRS.Queries.Request.ReviewsRequests;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using AutoMapper;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQueryRequest, Result<ReviewProfile>>
    {
        private readonly IReviewDao _reviewDao;
        private readonly IMapper _mapper;
        public GetReviewByIdQueryHandler(IReviewDao reviewDao, IMapper mapper)
        {
            _reviewDao = reviewDao;
            _mapper = mapper;
        }

        public async Task<Result<ReviewProfile>> Handle(GetReviewByIdQueryRequest request, CancellationToken cancellationToken)
        {
            Review? review = await _reviewDao.GetByIdAsync(Guid.Parse(request.reviewId));
            if (review != null)
            {
                var reviewProfile = _mapper.Map<ReviewProfile>(review);
                if (request.userId != null)
                {
                    var userId = Guid.Parse(request.userId);
                    reviewProfile.CurrentUserLiked = review?
                            .ReviewLikes.Any(like => like.UserId == userId) ?? false;
                }
                return new(reviewProfile) { statusCode = 200, message = "Ok" };
            }
            return new("Review not found") { statusCode = 404 };
        }
    }
}
