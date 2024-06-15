using amazon_backend.CQRS.Commands.OrderRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.OrderHandlers.CommandHandlers
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommandRequest, Result<string>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        public CancelOrderCommandHandler(DataContext dataContext, TokenService tokenService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
        }
        public async Task<Result<string>> Handle(CancelOrderCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            var user = decodeResult.data;
            Order? order = await _dataContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == Guid.Parse(request.OrderId) && o.Status != "Canceled");
            if (order != null && order.UserId == user.Id)
            {
                order.Status = "Canceled";
                if (order.OrderItems != null && order.OrderItems.Count != 0)
                {
                    foreach (var item in order.OrderItems)
                    {
                        Product? product = await _dataContext.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                        if (product != null)
                        {
                            product.Quantity += item.Quantity;
                        }
                    }
                }
                await _dataContext.SaveChangesAsync();
                return new("Ok") { statusCode = 200 };
            }
            return new("Order not found") { statusCode = 404 };
        }
    }
}
