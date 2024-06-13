using amazon_backend.CQRS.Commands.CategoryImageRequst;
using amazon_backend.CQRS.Commands.ReviewImageRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.AWSS3;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.CategoryImageHandlers.CommandHandlers
{
    public class RemoveCategoryImageCommandHandler : IRequestHandler<RemoveCategoryImageCommandRequst, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IS3Service _s3Service;

        public RemoveCategoryImageCommandHandler(DataContext dataContext, TokenService tokenService, IS3Service s3Service)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _s3Service = s3Service;
        }

        public async Task<Result<string>> Handle(RemoveCategoryImageCommandRequst request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
            if (!decodeResult.isSuccess)
            {
                return new() { message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;

            CategoryImage? categoryImage = await _dataContext
                .CategoryImages
                .AsNoTracking()
                .Include(pi => pi.Categories)
                .FirstOrDefaultAsync(ri => ri.Id == Guid.Parse(request.categoryImageId));
            if (categoryImage == null)
            {
                return new("Bad request") { data = "Image not found", statusCode = 404 };
            }
            var deleteResult = await _s3Service.DeleteFile(categoryImage.ImageUrl);
            if (!deleteResult)
            {
                return new("Internal Server Error") { data = "Deleting failed", statusCode = 500 };
            }
            if (categoryImage.Categories != null)
            {
                categoryImage.Categories.Clear();
            }
            _dataContext.Remove(categoryImage);
            await _dataContext.SaveChangesAsync();
            return new("Ok") { statusCode = 200 };
        }
    }
}
