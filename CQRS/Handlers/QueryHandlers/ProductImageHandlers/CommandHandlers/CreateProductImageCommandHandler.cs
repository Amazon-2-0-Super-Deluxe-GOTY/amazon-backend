using amazon_backend.CQRS.Commands.ReviewImageRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ProductImageProfiles;
using amazon_backend.Services.AWSS3;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewImageQueryHandlers.CommandHandlers
{
    public class CreateProductImageCommandHandler : IRequestHandler<CreateProductImageCommandRequest, Result<List<ProductImageProfile>>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;

        public CreateProductImageCommandHandler(DataContext dataContext, TokenService tokenService, IS3Service s3Service, IMapper mapper)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _s3Service = s3Service;
            _mapper = mapper;
        }

        public async Task<Result<List<ProductImageProfile>>> Handle(CreateProductImageCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
            if (!decodeResult.isSuccess)
            {
                return new() { message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            List<ProductImage> results = new();
            foreach (var item in request.productImages)
            {
                var reviewSlug = await _s3Service.UploadFile(item, "products");
                if (reviewSlug == null)
                {
                    return new("Upload failed") { statusCode = 500 };
                }
                ProductImage productImage = new()
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = reviewSlug,
                    CreatedAt = DateTime.Now
                };

                await _dataContext.AddAsync(productImage);
                await _dataContext.SaveChangesAsync();
                results.Add(productImage);
            }

            return new(_mapper.Map<List<ProductImageProfile>>(results)) { statusCode = 201, message = "Created" };
        }
    }
}
