using amazon_backend.CQRS.Commands.CartRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.CartItemProfiles;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.CartQueryHandlers.CommandHandlers
{
    public class AddNewItemToCartCommandHandler : IRequestHandler<AddNewItemToCartCommandRequest, Result<CartItemProfile>>
    {
        private readonly TokenService _tokenService;
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public AddNewItemToCartCommandHandler(TokenService tokenService, DataContext dataContext, IMapper mapper)
        {
            _tokenService = tokenService;
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<Result<CartItemProfile>> Handle(AddNewItemToCartCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }

            Product? product = await _dataContext.Products
                .Include(p => p.ProductImages)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.productId));
            if (product == null || product.Quantity == 0)
            {
                return new("Product not found") { statusCode = 404 };
            }

            Cart? cart = await _dataContext.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == decodeResult.data.Id);
            if (cart == null)
            {
                cart = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = decodeResult.data.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _dataContext.AddAsync(cart);
                await _dataContext.SaveChangesAsync();
            }
            if (cart.CartItems != null && cart.CartItems.Count != 0)
            {
                var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == product.Id);
                if (item != null)
                {
                    item.Quantity = (request.quantity + item.Quantity) > product.Quantity ? product.Quantity : request.quantity + item.Quantity;
                    _dataContext.Update(item);
                    await _dataContext.SaveChangesAsync();
                    return new("Ok") { statusCode = 200, data = _mapper.Map<CartItemProfile>(item) };
                }
            }
            var newItem = new CartItem()
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.quantity > product.Quantity ? product.Quantity : request.quantity,
                CreatedAt = DateTime.Now
            };
            await _dataContext.AddAsync(newItem);
            await _dataContext.SaveChangesAsync();
            return new("Created") { statusCode = 201, data = _mapper.Map<CartItemProfile>(newItem) };
        }
    }
}
