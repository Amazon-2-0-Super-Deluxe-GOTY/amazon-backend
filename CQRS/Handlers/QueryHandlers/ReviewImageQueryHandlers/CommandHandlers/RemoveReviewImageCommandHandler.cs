using amazon_backend.CQRS.Commands.ReviewImageRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.AWSS3;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewImageQueryHandlers.CommandHandlers
{
    public class RemoveReviewImageCommandHandler : IRequestHandler<RemoveReviewImageCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IS3Service _s3Service;

        public RemoveReviewImageCommandHandler(DataContext dataContext, TokenService tokenService, IS3Service s3Service)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _s3Service = s3Service;
        }

        public async Task<Result<string>> Handle(RemoveReviewImageCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;

            ReviewImage? reviewImage = await _dataContext
                .ReviewImages
                .AsNoTracking()
                .Include(ri=>ri.Review)
                .FirstOrDefaultAsync(ri => ri.Id == Guid.Parse(request.reviewImageId));
            if (reviewImage == null)
            {
                return new("Bad request") { data = "Image not found", statusCode = 404 };
            }
            if (reviewImage.UserId == user.Id || user.Role == "Admin")
            {
                var deleteResult = await _s3Service.DeleteFile(reviewImage.ImageUrl);
                if (!deleteResult)
                {
                    return new("Internal Server Error") { data = "Deleting failed", statusCode = 500 };
                }

                if (reviewImage.Review != null)
                {
                    reviewImage.Review.Clear();
                    await _dataContext.SaveChangesAsync();
                }

                _dataContext.Remove(reviewImage);
                await _dataContext.SaveChangesAsync();

                return new("Ok") { statusCode = 200 };
            }
            return new("Bad request") { data = "Image not found", statusCode = 404 };
        }
    }
}
