using amazon_backend.CQRS.Queries.Request.ProductRequests;
using amazon_backend.Data;
using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ProductHandlers.QueryHandlers
{
    public class GetFilterItemsQueryHandler : IRequestHandler<GetFilterItemsQueryRequest, Result<object>>
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<GetFilterItemsQueryHandler> _logger;
        public GetFilterItemsQueryHandler(DataContext dataContext, ILogger<GetFilterItemsQueryHandler> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }
        public async Task<Result<object>> Handle(GetFilterItemsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dataContext.Products.AsQueryable();
                if (request.categoryId.HasValue)
                {
                    Category? category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Id == request.categoryId.Value);
                    if (category == null)
                    {
                        return new("Category not found") { statusCode = 404 };
                    }
                    query = query.Where(p => p.CategoryId == request.categoryId.Value);
                    int count = await query.CountAsync();
                    if (count <= 0)
                    {
                        return new("Category is empty") { statusCode = 404 };
                    }
                }

                var queryResult = await query
                    .SelectMany(
                    p => p.ProductProperties.DefaultIfEmpty(),
                    (p, pp) => new
                    {
                        Product = p,
                        ProductProperty = pp
                    })
                    .Where(x => x.ProductProperty != null)
                    .GroupBy(x => new { x.ProductProperty.Key, x.ProductProperty.Value })
                    .Select(g => new
                    {
                        Key = g.Key.Key,
                        Value = g.Key.Value
                    })
                    .ToListAsync();
                if (queryResult != null)
                {
                    var maxPrice = await query.MaxAsync(p => p.Price * (1 - (p.DiscountPercent.HasValue ? p.DiscountPercent.Value : 0) / 100.0));
                    var minPrice = await query.MinAsync(p => p.Price * (1 - (p.DiscountPercent.HasValue ? p.DiscountPercent.Value : 0) / 100.0));
                    var filterItems = queryResult
                        .GroupBy(d => d.Key)
                        .ToDictionary(
                        g => g.Key,
                        g => g.Select(d => d.Value).ToList());
                    var resultData = new
                    {
                        filterItems = filterItems,
                        minPrice = (int)minPrice,
                        maxPrice = (int)Math.Ceiling(maxPrice)
                    };
                    return new("Ok") { data = resultData, statusCode = 200 };
                }
                return new("Filter items not found") { statusCode = 404 };
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex.Message);
                return new("Operation was canceled") { statusCode = 400 };
            }
            catch (DbUpdateException ex)
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
