using amazon_backend.CQRS.Commands.OrderRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.OrderProfiles;
using amazon_backend.Services.JWTService;
using amazon_backend.Services.Random;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.OrderHandlers.CommandHandlers
{
    public class NewOrderFromCartCommandHandler : IRequestHandler<NewOrderFromCartCommandRequest, Result<OrderProfile>>
    {
        private readonly TokenService _tokenService;
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly IRandomService _randomService;

        public NewOrderFromCartCommandHandler(TokenService tokenService, DataContext dataContext, IMapper mapper, IRandomService randomService)
        {
            _tokenService = tokenService;
            _dataContext = dataContext;
            _mapper = mapper;
            _randomService = randomService;
        }

        public async Task<Result<OrderProfile>> Handle(NewOrderFromCartCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            var user = decodeResult.data;
            Cart? cart = await _dataContext.Carts
                .Include(c => c.CartItems!)
                .ThenInclude(ci=>ci.Product)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (cart == null)
            {
                return new("Cart is empty") { statusCode = 404 };
            }

            if (cart.CartItems == null || cart.CartItems.Count == 0)
            {
                return new("Cart is empty") { statusCode = 404 };
            }

            var totalQuantity = cart.CartItems.Sum(ci => ci.Product!.Quantity);
            if (totalQuantity == 0)
            {
                for (int i = 0; i < cart.CartItems.Count; i++)
                {
                    _dataContext.Remove(cart.CartItems[i]);
                    await _dataContext.SaveChangesAsync();
                }
                return new("Cart is empty") { statusCode = 404 };
            }

            Order newOrder = new()
            {
                Id = Guid.NewGuid(),
                OrderNumber = _randomService.RandomNumberUseDate(),
                PaymentMethod = request.paymentMethod,
                Status = "Ordered",
                CreatedAt = DateTime.Now,
                UserId = user.Id
            };
            await _dataContext.AddAsync(newOrder);
            await _dataContext.SaveChangesAsync();
            DeliveryAddress deliveryAddress = new()
            {
                Id = Guid.NewGuid(),
                OrderId = newOrder.Id,
                Country = request.country,
                City = request.city,
                State = request.state,
                PostIndex = request.postIndex
            };
            await _dataContext.AddAsync(deliveryAddress);
            await _dataContext.SaveChangesAsync();

            foreach (var item in cart.CartItems)
            {
                var product = await _dataContext.Products
                    .Include(p => p.ProductImages)
                    .FirstAsync(p => p.Id == item.ProductId);
                var quantity = product.Quantity < item.Quantity ? product.Quantity : item.Quantity;
                if (quantity == 0)
                {
                    continue;
                }
                var price = product.Price * (1 - (product.DiscountPercent.HasValue ? product.DiscountPercent.Value : 0) / 100.0);
                string imageUrl = null!;
                if (product.ProductImages != null && product.ProductImages.Count != 0) imageUrl = product.ProductImages.First().ImageUrl;
                OrderItem newItem = new()
                {
                    Id = Guid.NewGuid(),
                    OrderId = newOrder.Id,
                    Quantity = quantity,
                    Price = price,
                    TotalPrice = Math.Round(price * quantity, 2),
                    Name = product.Name,
                    ProductId = product.Id,
                    ImageUrl = imageUrl
                };
                int newQuantity = 0;
                if (product.Quantity > quantity)
                {
                    newQuantity = product.Quantity - quantity == 0 ? 0 : product.Quantity - quantity;
                }
                else
                {
                    newQuantity = quantity - product.Quantity == 0 ? 0 : quantity - product.Quantity;
                }
                product.Quantity = newQuantity;
                await _dataContext.AddAsync(newItem);
                await _dataContext.SaveChangesAsync();
            }
            for (int i = 0; i < cart.CartItems.Count; i++)
            {
                _dataContext.Remove(cart.CartItems[i]);
            }
            await _dataContext.SaveChangesAsync();
            return new("Created") { statusCode = 201, data = _mapper.Map<OrderProfile>(newOrder) };
        }
    }
}
