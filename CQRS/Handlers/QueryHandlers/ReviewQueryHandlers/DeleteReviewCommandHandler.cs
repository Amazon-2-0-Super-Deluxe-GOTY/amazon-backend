using amazon_backend.CQRS.Commands.ReviewRequests;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommandRequest, Result<string>>
    {
        private readonly IReviewDao _reviewDao;
        private readonly TokenService _tokenService;

        public DeleteReviewCommandHandler(IReviewDao reviewDao, TokenService tokenService)
        {
            _reviewDao = reviewDao;
            _tokenService = tokenService;
        }
        public async Task<Result<string>> Handle(DeleteReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            var reviewId = Guid.Parse(request.reviewId);

            Review? review = await _reviewDao.GetByIdAsync(reviewId);
            if (review == null)
            {
                return new("Not found") { statusCode = 404 };
            }
            if (review.UserId == user.Id || user.Role == "Admin")
            {
                var result = await _reviewDao.DeleteAsync(reviewId);
                if (result)
                {
                    return new("Ok") { statusCode = 200 };
                }
                return new("See server logs") { statusCode = 500 };
            }
            return new("Not found") { statusCode = 404 };
        }
    }
}
