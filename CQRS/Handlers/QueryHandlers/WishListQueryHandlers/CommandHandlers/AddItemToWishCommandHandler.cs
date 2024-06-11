using amazon_backend.CQRS.Commands.WishListRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.WishListQueryHandlers.CommandHandlers
{
    public class AddItemToWishCommandHandler : IRequestHandler<AddItemToWishCommandRequest, Result<ProductCardProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;

        public AddItemToWishCommandHandler(DataContext dataContext, TokenService tokenSerivce, IMapper mapper)
        {
            _dataContext = dataContext;
            _tokenService = tokenSerivce;
            _mapper = mapper;
        }

        public async Task<Result<ProductCardProfile>> Handle(AddItemToWishCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            Product? product = await _dataContext.Products.FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.productId));
            if(product == null)
            {
                return new("Product not found") { statusCode = 400 };
            }
            var user = await _dataContext.Users.Include(u => u.WishedProducts).FirstAsync(u => u.Id == decodeResult.data.Id);
            if (user.WishedProducts == null)
            {
                user.WishedProducts = new();
            }
            if (user.WishedProducts.Contains(product))
            {
                return new("Product already exist in wishes") { statusCode = 400 };
            }

            user.WishedProducts.Add(product);
            await _dataContext.SaveChangesAsync();
            return new("Created") { statusCode = 201, data = _mapper.Map<ProductCardProfile>(product) };
        }
    }
}
