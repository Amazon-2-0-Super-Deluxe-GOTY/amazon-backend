using amazon_backend.CQRS.Commands.ReviewRequests;
using amazon_backend.Data.Entity;
using amazon_backend.Data;
using amazon_backend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using amazon_backend.Profiles.ReviewProfiles;
using AutoMapper;
using amazon_backend.Services.JWTService;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommandRequest, Result<ReviewProfile>>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;

        public UpdateReviewCommandHandler(DataContext dataContext, IMapper mapper, TokenService tokenService)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<Result<ReviewProfile>> Handle(UpdateReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;

            if (request.text == null && request.title == null && !request.rating.HasValue && request.reviewTagsIds == null
                && (request.reviewImagesIds == null || request.reviewImagesIds.Count == 0))
            {
                return new("No parameters for update") { statusCode = 400 };
            }

            Review? review = await _dataContext.Reviews
                .Include(r => r.ReviewTags)
                .Include(r => r.ReviewImages)
                .AsSplitQuery()
                .Where(r => r.Id == Guid.Parse(request.reviewId)).FirstOrDefaultAsync();
            if (review != null && review.UserId == user.Id)
            {
                if (!string.IsNullOrEmpty(request.text))
                {
                    review.Text = request.text;
                }
                if (!string.IsNullOrEmpty(request.title))
                {
                    review.Title = request.title;
                }
                if (request.rating.HasValue)
                {
                    review.Mark = (int)request.rating;
                }
                if (request.reviewImagesIds != null && request.reviewImagesIds.Count != 0)
                {
                    if (review.ReviewImages == null)
                    {
                        review.ReviewImages = new List<ReviewImage>();
                    }
                    if (review.ReviewImages.Count == 10)
                    {
                        return new("Maximum 10 images") { statusCode = 400 };
                    }
                    var canAddCount = (10 - review.ReviewImages.Count());
                    if (canAddCount > 0)
                    {
                        var count = 0;
                        foreach (var rImage in request.reviewImagesIds)
                        {
                            ReviewImage? image = await _dataContext.ReviewImages.Where(t => t.Id == Guid.Parse(rImage)).FirstOrDefaultAsync();
                            if (image != null)
                            {
                                if (!review.ReviewImages.Contains(image))
                                {
                                    review.ReviewImages.Add(image);
                                    count++;
                                }
                            }
                            if (canAddCount == count) break;
                        }
                    }
                }
                if (request.reviewTagsIds != null)
                {
                    if (review.ReviewTags == null)
                    {
                        review.ReviewTags = new List<ReviewTag>();
                    }
                    review.ReviewTags.Clear();
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
                return new(_mapper.Map<ReviewProfile>(review)) { statusCode = 200 };
            }

            return new("Review not found") { statusCode = 404 };
        }
    }
}
