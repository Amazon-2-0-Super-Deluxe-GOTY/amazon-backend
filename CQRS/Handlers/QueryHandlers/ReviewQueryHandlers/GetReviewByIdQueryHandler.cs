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
                var reviewView = _mapper.Map<ReviewProfile>(review);
                return new(reviewView);
            }
            return new("Review not found");
        }
    }
}
