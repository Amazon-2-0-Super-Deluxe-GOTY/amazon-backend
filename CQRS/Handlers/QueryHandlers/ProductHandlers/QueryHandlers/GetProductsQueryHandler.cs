using amazon_backend.CQRS.Queries.Request.ProductRequests;
using amazon_backend.Data;
using amazon_backend.Models;
using amazon_backend.Profiles.ProductImageProfiles;
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

        private readonly string FILTER_SEPARATOR;
        private readonly char RATING_SEPARATOR;
        private readonly char DASH_DELIMETER;

        public GetProductsQueryHandler(IMapper mapper, DataContext dataContext, IHttpContextAccessor httpContextAccessor, ILogger<GetProductsQueryHandler> logger)
        {
            _mapper = mapper;
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            FILTER_SEPARATOR = "--";
            RATING_SEPARATOR = ',';
            DASH_DELIMETER = '-';
        }

        public async Task<Result<List<ProductCardProfile>>> Handle(GetProductsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var productsQuery = _dataContext.Products
                    .Include(p => p.Reviews)
                    .Include(p => p.ProductProperties)
                    .AsSplitQuery()
                    .AsQueryable();

                if (request.categoryId.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.CategoryId == request.categoryId);
                }

                if (request.discount.HasValue && request.discount.Value == true)
                {
                    productsQuery = productsQuery.Where(p => p.DiscountPercent.HasValue && p.DiscountPercent.Value > 0);
                }

                if (request.searchQuery != null)
                {
                    productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(request.searchQuery.Replace("+", " ")));
                }

                if (request.price != null)
                {
                    var parsedPrice = request.price.Split(DASH_DELIMETER);
                    int minPrice = int.Parse(parsedPrice[0]);
                    int maxPrice = int.Parse(parsedPrice[1]);
                    productsQuery = productsQuery
                        .Where(p => (p.Price * (1 - (p.DiscountPercent.HasValue ? p.DiscountPercent.Value : 0) / 100.0)) >= minPrice &&
                        (p.Price * (1 - (p.DiscountPercent.HasValue ? p.DiscountPercent.Value : 0) / 100.0)) <= maxPrice);
                }
                if (request.rating != null)
                {
                    var rating = new List<int>();
                    var parsedRating = request.rating.Split(RATING_SEPARATOR);
                    foreach (var item in parsedRating)
                    {
                        rating.Add(int.Parse(item));
                    }
                    productsQuery = productsQuery
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
                    // get filter items from request
                    // exmp: Brand=Apple--Samsung&Color=Red--Green
                    var queryParams = httpContext.Request.Query;
                    List<string> propValues = new();
                    List<string> propKeys = new();
                    if (queryParams != null)
                    {
                        // parse filter items
                        var pProps = await _dataContext.ProductProperties.AsNoTracking().ToListAsync();
                        foreach (var item in queryParams)
                        {
                            var prop = pProps.Where(pp => pp.Key.ToLower() == item.Key.ToLower()).FirstOrDefault();
                            if (prop != null)
                            {
                                var valueArr = item.Value.ToString().Split(FILTER_SEPARATOR)
                                    .Select((v) =>
                                    {
                                        return v.Replace("+", " ").ToLower();
                                    }).ToArray();
                                propValues.AddRange(valueArr);
                                propKeys.Add(item.Key);
                            }
                        }
                        // filter products
                        if (propValues.Count != 0 && propKeys.Count != 0)
                        {
                            productsQuery = productsQuery
                                    .Where(p => p.ProductProperties
                                    .Any(pp => propKeys.Contains(pp.Key.ToLower())));

                            productsQuery = productsQuery
                                .Where(p => p.ProductProperties
                                .Any(pp => propValues.Contains(pp.Value.ToLower())));
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
                                .Include(p => p.ProductImages)
                                .Select(p => new
                                {
                                    Product = p,
                                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Mark) : 0
                                })
                                .OrderByDescending(p => p.AverageRating)
                                .Select(p => p.Product);
                            break;
                        case "cheap":
                            productsQuery = productsQuery.OrderBy(p => p.Price * (1 - (p.DiscountPercent.HasValue ? p.DiscountPercent.Value : 0) / 100.0));
                            break;
                        case "exp":
                            productsQuery = productsQuery.OrderByDescending(p => p.Price * (1 - (p.DiscountPercent.HasValue ? p.DiscountPercent.Value : 0) / 100.0));
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
                    foreach (var item in productProfiles)
                    {
                        var product = await _dataContext.Products.Include(p => p.ProductImages).FirstAsync(p => p.Id == item.Id);
                        item.ProductImages = _mapper.Map<List<ProductImageCardProfile>>(product.ProductImages);
                    }
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
