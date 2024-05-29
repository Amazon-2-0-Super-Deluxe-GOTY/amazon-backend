using Amazon.S3;
using amazon_backend.CQRS.Commands.ReviewRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using amazon_backend.Services.AWSS3;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommandRequest, Result<ReviewProfile>>
    {
        private readonly IReviewDao _reviewDao;
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;

        public CreateReviewCommandHandler(IReviewDao reviewDao, DataContext dataContext, TokenService tokenService, IMapper mapper)
        {
            _reviewDao = reviewDao;
            _dataContext = dataContext;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        public async Task<Result<ReviewProfile>> Handle(CreateReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            var productId = Guid.Parse(request.productId);
            Product? product = await _dataContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                return new("Product not found") { statusCode = 404 };
            }
            Review newReview = new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ProductId = Guid.Parse(request.productId),
                Text = request.text,
                CreatedAt = DateTime.Now,
                Mark = request.rating,
            };
            if (request.reviewTagsIds != null && request.reviewTagsIds.Count != 0)
            {
                newReview.ReviewTags = new List<ReviewTag>();
                foreach (var rTag in request.reviewTagsIds)
                {
                    ReviewTag? tag = await _dataContext.ReviewTags.FirstOrDefaultAsync(t => t.Id == Guid.Parse(rTag));
                    if (tag != null)
                    {
                        if (!newReview.ReviewTags.Contains(tag))
                        {
                            newReview.ReviewTags.Add(tag);
                        }
                    }
                }
            }
            if(request.reviewImagesIds!=null&&request.reviewImagesIds.Count != 0)
            {
                newReview.ReviewImages= new List<ReviewImage>();
                foreach(var rImage in request.reviewImagesIds)
                {
                    ReviewImage? image = await _dataContext.ReviewImages.FirstOrDefaultAsync(i => i.Id == Guid.Parse(rImage));
                    if (image != null)
                    {
                        newReview.ReviewImages.Add(image);
                    }
                }
            }
            var result = await _reviewDao.AddAsync(newReview);
            return new("Created") { data = _mapper.Map<ReviewProfile>(newReview), statusCode = 201 };
        }
    }
}
