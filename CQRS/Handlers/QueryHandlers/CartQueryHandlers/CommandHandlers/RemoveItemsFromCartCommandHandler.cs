using amazon_backend.CQRS.Commands.CartRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.CartQueryHandlers.CommandHandlers
{
    public class RemoveItemsFromCartCommandHandler : IRequestHandler<RemoveItemsFromCartCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;

        public RemoveItemsFromCartCommandHandler(DataContext dataContext, TokenService tokenService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
        }

        public async Task<Result<string>> Handle(RemoveItemsFromCartCommandRequest request, CancellationToken cancellationToken)
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

            foreach (var item in request.cartItemIds)
            {
                CartItem? cartItem = await _dataContext.CartItems.FirstOrDefaultAsync(ci => ci.Id == Guid.Parse(item));
                if (cartItem != null)
                {
                    if (cart.CartItems.Contains(cartItem))
                    {
                        _dataContext.Remove(cartItem);
                        await _dataContext.SaveChangesAsync();
                    }
                }
            }
            return new("Ok") { statusCode = 200 };
        }
    }
}
