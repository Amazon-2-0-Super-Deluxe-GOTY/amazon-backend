using amazon_backend.CQRS.Queries.Request.ProductRequests;
using amazon_backend.Data;
using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ProductHandlers.QueryHandlers
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQueryRequest, Result<List<ProductCardProfile>>>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetProductsQueryHandler> _logger;

        private readonly char COMMA_DELIMETER = ',';
        private readonly char DASH_DELIMETER = '-';

        public GetProductsQueryHandler(IMapper mapper, DataContext dataContext, IHttpContextAccessor httpContextAccessor, ILogger<GetProductsQueryHandler> logger)
        {
            _mapper = mapper;
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<Result<List<ProductCardProfile>>> Handle(GetProductsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var productsQuery = _dataContext.Products
                    .Include(p => p.Reviews)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductProperties)
                    .AsQueryable();

                if (request.categoryId.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.CategoryId == request.categoryId);
                }

                if (request.discount.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.DiscountPercent != 0);
                }

                if (request.searchQuery != null)
                {
                    productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(request.searchQuery));
                }

                if (request.price != null)
                {
                    var parsedPrice = request.price.Split(DASH_DELIMETER);
                    int minPrice = int.Parse(parsedPrice[0]);
                    int maxPrice = int.Parse(parsedPrice[1]);
                    productsQuery = productsQuery.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
                }
                if (request.rating != null)
                {
                    var rating = new List<int>();
                    var parsedRating = request.rating.Split(COMMA_DELIMETER);
                    foreach (var item in parsedRating)
                    {
                        rating.Add(int.Parse(item));
                    }
                    productsQuery = productsQuery
                        .Include(p => p.Reviews)
                        .AsSplitQuery()
                        .Select(p => new
                        {
                            Product = p,
                            Avg = p.Reviews.Any() ? (int)Math.Round(p.Reviews.Average(r => r.Mark)) : 0
                        })
                        .Where(p => rating.Contains(p.Avg))
                        .Select(p => p.Product);
                }
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    var queryParams = httpContext.Request.Query;
                    if (queryParams != null)
                    {
                        var pProps = await _dataContext.ProductProperties.AsNoTracking().ToListAsync();
                        Dictionary<string, string[]> filterItems = new();
                        foreach (var item in queryParams)
                        {
                            var prop = pProps.Where(pp => pp.Key.ToLower() == item.Key.ToLower()).FirstOrDefault();
                            if (prop != null)
                            {
                                filterItems.Add(item.Key, item.Value.ToString().Split(COMMA_DELIMETER)
                                    .Select((v) =>
                                    {
                                        return v.Replace("+", " ").ToLower();
                                    }).ToArray());
                            }
                        }
                        if (filterItems.Count != 0)
                        {
                            foreach (var item in filterItems)
                            {
                                productsQuery = productsQuery
                                    .Where(p => p.ProductProperties
                                    .Any(pp => pp.Key.ToLower() == item.Key.ToLower() && item.Value.Contains(pp.Value.ToLower())));
                            }
                        }
                    }
                }
                if (request.orderBy != null)
                {
                    switch (request.orderBy)
                    {
                        case "date":
                            productsQuery = productsQuery.OrderByDescending(p => p.CreatedAt);
                            break;
                        case "rate":
                            productsQuery = productsQuery
                                .Include(p => p.Reviews)
                                .AsSplitQuery()
                                .Select(p => new
                                {
                                    Product = p,
                                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Mark) : 0
                                })
                                .OrderByDescending(p => p.AverageRating)
                                .Select(p => p.Product);
                            break;
                        case "cheap":
                            productsQuery = productsQuery.OrderByDescending(p => p.Price);
                            break;
                        case "exp":
                            productsQuery = productsQuery.OrderBy(p => p.Price);
                            break;
                        default:
                            productsQuery = productsQuery.OrderByDescending(p => p.CreatedAt);
                            break;
                    }
                }
                else
                {
                    productsQuery = productsQuery.OrderByDescending(p => p.CreatedAt);
                }

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
                return new("Products not found") { statusCode = 404 };
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex.Message);
                return new("See server logs") { statusCode = 500 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new("See server logs") { statusCode = 500 };
            }
        }
    }
}
