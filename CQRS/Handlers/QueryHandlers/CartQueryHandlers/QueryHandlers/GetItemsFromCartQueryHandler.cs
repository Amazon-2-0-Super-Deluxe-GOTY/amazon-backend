using amazon_backend.CQRS.Queries.Request.CartRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.CartItemProfiles;
using amazon_backend.Profiles.CartProfiles;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.CartQueryHandlers.QueryHandlers
{
    public class GetItemsFromCartQueryHandler : IRequestHandler<GetItemsFromCartQueryRequest, Result<CartProfile>>
    {
        private readonly TokenService _tokenService;
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public GetItemsFromCartQueryHandler(TokenService tokenService, DataContext dataContext, IMapper mapper)
        {
            _tokenService = tokenService;
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<Result<CartProfile>> Handle(GetItemsFromCartQueryRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            var user = decodeResult.data;

            Cart? cart = await _dataContext.Carts
                .Include(c => c.CartItems!)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.ProductImages)
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

            var result = cart.CartItems.OrderBy(ci => ci.CreatedAt)
                .Skip((request.pageIndex - 1) * request.pageSize)
                .Take(request.pageSize).ToList();

            if (result != null && result.Count != 0)
            {
                var cartProfile = _mapper.Map<CartProfile>(cart);
                cartProfile.CartItems = _mapper.Map<List<CartItemProfile>>(result);
                int totalCount = result.Count;
                int pagesCount = (int)Math.Ceiling(totalCount / (double)request.pageSize);
                return new(cartProfile, pagesCount) { statusCode = 200, message = "Ok" };
            }
            return new("Cart is empty") { statusCode = 404 };
        }
    }
}
