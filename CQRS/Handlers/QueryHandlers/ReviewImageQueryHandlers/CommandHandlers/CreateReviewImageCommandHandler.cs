using amazon_backend.CQRS.Commands.ReviewImageRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewImageProfiles;
using amazon_backend.Services.AWSS3;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewImageQueryHandlers.CommandHandlers
{
    public class CreateReviewImageCommandHandler : IRequestHandler<CreateReviewImageCommandRequest, Result<List<ReviewImageProfile>>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;

        public CreateReviewImageCommandHandler(DataContext dataContext, TokenService tokenService, IS3Service s3Service, IMapper mapper)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _s3Service = s3Service;
            _mapper = mapper;
        }

        public async Task<Result<List<ReviewImageProfile>>> Handle(CreateReviewImageCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            List<ReviewImage> results = new();
            foreach (var item in request.reviewImages)
            {
                var reviewSlug = await _s3Service.UploadFile(item, "reviews");
                if (reviewSlug == null)
                {
                    return new("Upload failed") { statusCode = 500 };
                }
                ReviewImage reviewImage = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    ImageUrl = reviewSlug,
                    CreatedAt = DateTime.Now
                };
                await _dataContext.AddAsync(reviewImage);
                await _dataContext.SaveChangesAsync();
                results.Add(reviewImage);
            }

            return new(_mapper.Map<List<ReviewImageProfile>>(results)) { statusCode = 201, message = "Created" };
        }
    }
}
