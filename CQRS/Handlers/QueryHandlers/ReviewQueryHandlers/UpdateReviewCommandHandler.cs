using amazon_backend.CQRS.Commands.ReviewRequests;
using amazon_backend.Data.Entity;
using amazon_backend.Data;
using amazon_backend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using amazon_backend.Services.AWSS3;
using Amazon.S3;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class UpdateReviewCommandHandler:IRequestHandler<UpdateReviewCommandRequest, Result<Guid>>
    {
        private readonly IReviewDao _reviewDao;
        private readonly DataContext _dataContext;
        private readonly IS3Service _s3Service;
        private readonly ILogger<UpdateReviewCommandHandler> _logger;
        public UpdateReviewCommandHandler(IReviewDao reviewDao, DataContext dataContext, IS3Service s3Service, ILogger<UpdateReviewCommandHandler> logger)
        {
            _reviewDao = reviewDao;
            _dataContext = dataContext;
            _s3Service = s3Service;
            _logger = logger;
        }
        public async Task<Result<Guid>> Handle(UpdateReviewCommandRequest request, CancellationToken cancellationToken)
        {
            if(request.text==null&&!request.rating.HasValue&&(request.reviewTagsIds==null|| request.reviewTagsIds.Count == 0)
                && (request.reviewImages == null || request.reviewImages.Count == 0))
            {
                return new("No parameters for update") { statusCode = 400 };
            }
            try
            {
                Review? review = await _dataContext.Reviews
                    .Include(r => r.ReviewTags)
                    .Include(r => r.ReviewImages)
                    .AsSplitQuery()
                    .Where(r => r.Id == Guid.Parse(request.reviewId)).FirstOrDefaultAsync();
                if (review != null)
                {
                    if (!string.IsNullOrEmpty(request.text))
                    {
                        review.Text = request.text;
                    }
                    if (request.rating.HasValue)
                    {
                        review.Mark = (int)request.rating;
                    }
                    if (request.reviewImages != null && request.reviewImages.Count != 0)
                    {
                        if (review.ReviewImages != null && review.ReviewImages.Count != 0)
                        {
                            foreach (var image in review.ReviewImages)
                            {
                                var result = await _reviewDao.DeleteReviewImageAsync(image.Id);
                                if (result)
                                {
                                    await _s3Service.DeleteFile(image.ImageUrl);
                                }
                            }
                        }
                        var imagesPaths = await _s3Service.UploadFilesFromRange(request.reviewImages, "reviews");
                        if (imagesPaths != null)
                        {
                            foreach (var imagePath in imagesPaths)
                            {
                                ReviewImage reviewImage = new();
                                reviewImage.Id = Guid.NewGuid();
                                reviewImage.ReviewId = review.Id;
                                reviewImage.ImageUrl = imagePath;
                                reviewImage.CreatedAt = DateTime.Now;
                                await _dataContext.AddAsync(reviewImage);
                                await _dataContext.SaveChangesAsync();
                            }
                        }
                    }
                    if (request.reviewTagsIds != null && request.reviewTagsIds.Count != 0)
                    {
                        if (review.ReviewTags != null && review.ReviewTags.Count != 0)
                        {
                            review.ReviewTags.Clear();
                        }
                        review.ReviewTags = new List<ReviewTag>();
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
                    return new(review.Id) { statusCode = 200 };
                }
            }
            catch(AmazonS3Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            catch(DbUpdateException ex)
            {
                _logger.LogError(ex.Message);
            }
            return new("See server logs") { statusCode = 500 };
        }
    }
}
