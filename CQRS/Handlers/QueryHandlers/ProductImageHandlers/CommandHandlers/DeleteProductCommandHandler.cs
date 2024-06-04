using amazon_backend.CQRS.Commands.ProductCommands;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.AWSS3;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ProductImageHandlers.CommandHandlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IReviewDao _reviewDao;
        private readonly IS3Service _s3Service;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(DataContext dataContext, TokenService tokenService, IReviewDao reviewDao, IS3Service s3Service, ILogger<DeleteProductCommandHandler> logger)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _reviewDao = reviewDao;
            _s3Service = s3Service;
            _logger = logger;
        }
        public async Task<Result<string>> Handle(DeleteProductCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }

            try
            {
                Product? product = await _dataContext.Products
                    .Include(p => p.Reviews)
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.productId));
                if (product == null)
                {
                    return new("Product not found") { statusCode = 404 };
                }

                if (product.Reviews != null && product.Reviews.Count != 0)
                {
                    for (int i = 0; i < product.Reviews.Count; i++)
                    {
                        await _reviewDao.DeleteAsync(product.Reviews[i].Id);
                    }
                }
                if (product.ProductImages != null && product.ProductImages.Count != 0)
                {
                    for (int i = 0; i < product.ProductImages.Count; i++)
                    {
                        var result = await _s3Service.DeleteFile(product.ProductImages[i].ImageUrl);
                        if (result)
                        {
                            _dataContext.Remove(product.ProductImages[i]);
                            await _dataContext.SaveChangesAsync();
                        }
                    }
                }
                _dataContext.Remove(product);
                await _dataContext.SaveChangesAsync();
                return new("Ok") { statusCode = 200 };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return new("See server logs") { statusCode = 500 };
            }
        }
    }
}
