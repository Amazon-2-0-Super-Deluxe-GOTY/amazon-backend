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
    public class DeleteReviewImageCommandHandler : IRequestHandler<DeleteReviewImageCommandRequest, Result<Guid>>
    {
        private readonly DataContext _dataContext;
        private readonly IS3Service _s3Service;
        private readonly ILogger<DeleteReviewImageCommandHandler> _logger;
        public DeleteReviewImageCommandHandler(DataContext dataContext, IS3Service s3Service, ILogger<DeleteReviewImageCommandHandler> logger)
        {
            _dataContext = dataContext;
            _s3Service = s3Service;
            _logger = logger;
        }
        public async Task<Result<Guid>> Handle(DeleteReviewImageCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                ReviewImage? image = await _dataContext.ReviewImages.FirstOrDefaultAsync(i => i.Id == Guid.Parse(request.imageId));
                if(image == null)
                {
                    return new("Review image not found") { statusCode = 404 };
                }
                var result = await _s3Service.DeleteFile(image.ImageUrl);
                _dataContext.Remove(image);
                await _dataContext.SaveChangesAsync(cancellationToken);
                return new(image.Id);
            }
            catch(ArgumentException ex)
            {
                _logger.LogError(ex.Message);
            }
            catch(OperationCanceledException ex)
            {
                _logger.LogError(ex.Message);
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return new("See server logs") { statusCode = 500 };
        }
    }
}

