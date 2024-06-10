using amazon_backend.CQRS.Commands.CategoryImageRequst;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.CategoryProfiles;
using amazon_backend.Profiles.ProductImageProfiles;
using amazon_backend.Services.AWSS3;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.CategoryImageHandlers.CommandHandlers
{
    public class CreateCategoryImageCommandHandler : IRequestHandler<CreateCategoryImageCommandRequst, Result<List<CategoryImageProfile>>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;

        public CreateCategoryImageCommandHandler(DataContext dataContext, TokenService tokenService, IS3Service s3Service, IMapper mapper)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _s3Service = s3Service;
            _mapper = mapper;
        }

        public async Task<Result<List<CategoryImageProfile>>> Handle(CreateCategoryImageCommandRequst request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
            if (!decodeResult.isSuccess)
            {
                return new Result<List<CategoryImageProfile>> { message = decodeResult.message, statusCode = decodeResult.statusCode };
            }

            var imageSlug = await _s3Service.UploadFile(request.categoryImages, "categories");
            if (imageSlug == null)
            {
                return new Result<List<CategoryImageProfile>> { message = "Upload failed", statusCode = 500 };
            }

            var categoryImage = new CategoryImage
            {
                Id = Guid.NewGuid(),
                ImageUrl = imageSlug,
                CategoryId = request.CategoryId
            };

            await _dataContext.CategoryImages.AddAsync(categoryImage);
            await _dataContext.SaveChangesAsync(cancellationToken);

            var categoryImageProfile = _mapper.Map<CategoryImageProfile>(categoryImage);

            
            return new Result<List<CategoryImageProfile>>
            {
                data = new List<CategoryImageProfile> { categoryImageProfile },
                statusCode = 201,
                message = "Created"
            };
        }
    }
}


