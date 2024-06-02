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
                Category? category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Id == request.categoryId);
                if (category == null)
                {
                    return new("Category not found") { statusCode = 404 };
                }
                var queryResult = await _dataContext.Products
                    .Where(p => p.CategoryId == category.Id)
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
                    var maxPrice = await _dataContext.Products.Where(p => p.CategoryId == category.Id).MaxAsync(p => p.Price);
                    var minPrice = await _dataContext.Products.Where(p => p.CategoryId == category.Id).MinAsync(p => p.Price);
                    var filterItems = queryResult
                        .GroupBy(d => d.Key)
                        .ToDictionary(
                        g => g.Key,
                        g => g.Select(d => d.Value).ToList());
                    var resultData = new
                    {
                        filterItems = filterItems,
                        minPrice = minPrice,
                        maxPrice = maxPrice
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
