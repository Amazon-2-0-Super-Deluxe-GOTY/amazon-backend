using amazon_backend.CQRS.Queries.Request.ReviewsRequests;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQueryRequest, Result<ReviewProfile>>
    {
        private readonly IReviewDao _reviewDao;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TokenService _tokenService;

        public GetReviewByIdQueryHandler(IReviewDao reviewDao, IMapper mapper, IHttpContextAccessor httpContextAccessor, TokenService tokenService)
        {
            _reviewDao = reviewDao;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }

        public async Task<Result<ReviewProfile>> Handle(GetReviewByIdQueryRequest request, CancellationToken cancellationToken)
        {
            Review? review = await _reviewDao.GetByIdAsync(Guid.Parse(request.reviewId));
            if (review != null)
            {
                var reviewProfile = _mapper.Map<ReviewProfile>(review);
                string? token = null;
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    token = httpContext.Request.Cookies["jwt"];
                }
                if (token != null)
                {
                    var decodeResult = await _tokenService.DecodeToken("Bearer " + token, false);
                    await Console.Out.WriteLineAsync($"{decodeResult.message}");
                    if (decodeResult.isSuccess)
                    {
                        var user = decodeResult.data;
                        reviewProfile.CurrentUserLiked = review?
                            .ReviewLikes.Any(like => like.UserId == user.Id) ?? false;
                    }
                }
                return new(reviewProfile) { statusCode = 200, message = "Ok" };
            }
            return new("Review not found") { statusCode = 404 };
        }
    }
}
