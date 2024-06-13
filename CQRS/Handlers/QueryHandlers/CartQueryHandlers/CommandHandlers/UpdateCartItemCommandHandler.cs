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
    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommandRequest, Result<CartItemProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;

        public UpdateCartItemCommandHandler(DataContext dataContext, TokenService tokenService, IMapper mapper)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<Result<CartItemProfile>> Handle(UpdateCartItemCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            var user = decodeResult.data;
            Cart? cart = await _dataContext.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                return new("Cart is empty") { statusCode = 404 };
            }

            if (cart.CartItems == null || cart.CartItems.Count == 0)
            {
                return new("Cart is empty") { statusCode = 404 };
            }

            CartItem? cartItem = await _dataContext.CartItems.Include(ci => ci.Product).ThenInclude(p => p.ProductImages).FirstOrDefaultAsync(ci => ci.Id == Guid.Parse(request.cartItemId));
            if(cartItem == null)
            {
                return new("Item not found") { statusCode = 404 };
            }

            if (cart.CartItems.Contains(cartItem))
            {
                cartItem.Quantity = request.quantity > cartItem.Product!.Quantity ? cartItem.Product!.Quantity : request.quantity;
                await _dataContext.SaveChangesAsync();
                return new("Ok") { statusCode = 200, data = _mapper.Map<CartItemProfile>(cartItem) };
            }
            return new("Item not found") { statusCode = 404 };
        }
    }
}
