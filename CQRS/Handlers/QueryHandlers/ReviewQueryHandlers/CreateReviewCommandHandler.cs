using Amazon.S3;
using amazon_backend.CQRS.Commands.ReviewRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.AWSS3;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommandRequest, Result<Guid>>
    {
        private readonly IReviewDao _reviewDao;
        private readonly DataContext _dataContext;
        private readonly IS3Service _s3Service;
        private readonly ILogger<CreateReviewCommandHandler> _logger;
        public CreateReviewCommandHandler(IReviewDao reviewDao, DataContext dataContext, IS3Service s3Service, ILogger<CreateReviewCommandHandler> logger)
        {
            _reviewDao = reviewDao;
            _dataContext = dataContext;
            _s3Service = s3Service;
            _logger = logger;
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
            if (request.reviewImages != null)
            {
                try
                {
                    var imagesPaths = await _s3Service.UploadFilesFromRange(request.reviewImages,"reviews");
                    if (imagesPaths != null)
                    {
                        foreach (var imagePath in imagesPaths)
                        {
                            ReviewImage reviewImage = new();
                            reviewImage.Id = Guid.NewGuid();
                            reviewImage.ReviewId = newReview.Id;
                            reviewImage.ImageUrl = imagePath;
                            reviewImage.CreatedAt = DateTime.Now;
                            await _dataContext.AddAsync(reviewImage);
                            await _dataContext.SaveChangesAsync();
                        }
                    }
                }
                catch (AmazonS3Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            if (result)
            {
                return new(newReview.Id);
            }
            return new("See server logs");
        }
    }
}
