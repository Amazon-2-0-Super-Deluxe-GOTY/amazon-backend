using amazon_backend.CQRS.Queries.Request.WishListRequests;
using amazon_backend.Data;
using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.WishListQueryHandlers.QueryHandlers
{
    public class GetWishListQueryHandler : IRequestHandler<GetWishListQueryRequest, Result<List<ProductCardProfile>>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;

        public GetWishListQueryHandler(DataContext dataContext, TokenService tokenService, IMapper mapper)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<Result<List<ProductCardProfile>>> Handle(GetWishListQueryRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            var user = decodeResult.data;

            var wishlistedProductIds = await _dataContext.Products
                .Include(p => p.WishListers)
                .Where(p => p.WishListers.Contains(user))
                .Select(p => p.Id)
                .ToListAsync();

            if (wishlistedProductIds == null || wishlistedProductIds.Count == 0)
            {
                return new("Items not found") { statusCode = 404 };
            }

            var productsQuery = _dataContext.Products
                    .Include(p => p.Reviews)
                    .Include(p => p.ProductImages)
                    .Where(p => wishlistedProductIds.Contains(p.Id));

            if (request.searchQuery != null)
            {
                productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(request.searchQuery.Replace("+", " ")));
            }
            productsQuery = productsQuery.OrderByDescending(p => p.Id);

            var products = await productsQuery
                   .Skip(request.pageSize * (request.pageIndex - 1))
                   .Take(request.pageSize)
                   .ToListAsync();
            if (products != null && products.Count != 0)
            {
                var productProfiles = _mapper.Map<List<ProductCardProfile>>(products);
                int totalCount = await productsQuery.CountAsync();
                int pagesCount = (int)Math.Ceiling(totalCount / (double)request.pageSize);
                return new(productProfiles, pagesCount) { statusCode = 200 };
            }

            return new("Items not found") { statusCode = 404 };
        }
    }
}
