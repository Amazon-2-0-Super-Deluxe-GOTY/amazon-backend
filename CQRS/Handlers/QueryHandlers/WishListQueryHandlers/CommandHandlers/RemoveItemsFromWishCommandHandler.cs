using amazon_backend.CQRS.Commands.WishListRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.WishListQueryHandlers.CommandHandlers
{
    public class RemoveItemsFromWishCommandHandler : IRequestHandler<RemoveItemsFromWishCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;

        public RemoveItemsFromWishCommandHandler(DataContext dataContext, TokenService tokenService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
        }

        public async Task<Result<string>> Handle(RemoveItemsFromWishCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            var user = await _dataContext.Users.Include(u => u.WishedProducts).FirstAsync(u => u.Id == decodeResult.data.Id);
            if (user.WishedProducts == null || user.WishedProducts.Count == 0)
            {
                return new("Wish list is empty") { statusCode = 400 };
            }
            foreach (var item in request.productIds)
            {
                Product? product = await _dataContext.Products.FirstOrDefaultAsync(p => p.Id == Guid.Parse(item));
                if (product != null)
                {
                    user.WishedProducts.Remove(product);
                }
            }
            await _dataContext.SaveChangesAsync();
            return new("Ok") { statusCode = 200 };
        }
    }
}
