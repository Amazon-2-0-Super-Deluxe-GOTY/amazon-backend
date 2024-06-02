using amazon_backend.CQRS.Commands.ReviewRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class LikeReviewCommandHandler : IRequestHandler<LikeReviewCommandRequest, Result<string>>
    {
        private readonly TokenService _tokenService;
        private readonly DataContext _dataContext;
        public LikeReviewCommandHandler(TokenService tokenService, DataContext dataContext)
        {
            _tokenService = tokenService;
            _dataContext = dataContext;
        }
        public async Task<Result<string>> Handle(LikeReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            Review? review = await _dataContext.Reviews.AsNoTracking().FirstOrDefaultAsync(r => r.Id == Guid.Parse(request.reviewId));
            if (review == null)
            {
                return new("Review not found") { statusCode = 404 };
            }
            ReviewLike? reviewLike = await _dataContext.ReviewLikes.FirstOrDefaultAsync(rl => rl.UserId == user.Id && rl.ReviewId == review.Id);
            if (reviewLike == null)
            {
                ReviewLike newLike = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    ReviewId = review.Id
                };
                await _dataContext.AddAsync(newLike);
                await _dataContext.SaveChangesAsync();
                return new("Created") { statusCode = 201 };
            }
            _dataContext.Remove(reviewLike);
            await _dataContext.SaveChangesAsync();
            return new("Ok") { statusCode = 200 };
        }
    }
}
