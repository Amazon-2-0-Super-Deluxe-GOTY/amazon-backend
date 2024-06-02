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
    public class RemoveProductImageCommandHandler : IRequestHandler<RemoveProductImageCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IS3Service _s3Service;

        public RemoveProductImageCommandHandler(DataContext dataContext, TokenService tokenService, IS3Service s3Service)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _s3Service = s3Service;
        }

        public async Task<Result<string>> Handle(RemoveProductImageCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
            if (!decodeResult.isSuccess)
            {
                return new() { message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;

            ProductImage? productImage = await _dataContext
                .ProductImages
                .AsNoTracking()
                .Include(pi => pi.Products)
                .FirstOrDefaultAsync(ri => ri.Id == Guid.Parse(request.productImageId));
            if (productImage == null)
            {
                return new("Bad request") { data = "Image not found", statusCode = 404 };
            }
            var deleteResult = await _s3Service.DeleteFile(productImage.ImageUrl);
            if (!deleteResult)
            {
                return new("Internal Server Error") { data = "Deleting failed", statusCode = 500 };
            }
            if (productImage.Products != null)
            {
                productImage.Products.Clear();
            }
            _dataContext.Remove(productImage);
            await _dataContext.SaveChangesAsync();
            return new("Ok") { statusCode = 200 };
        }
    }
}
