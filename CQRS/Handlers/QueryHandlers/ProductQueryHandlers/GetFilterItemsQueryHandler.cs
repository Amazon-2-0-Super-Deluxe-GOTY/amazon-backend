using amazon_backend.CQRS.Queries.Request;
using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ProductQueryHandlers
{
    public class GetFilterItemsQueryHandler : IRequestHandler<GetFilterItemsQueryRequest, Result<List<FilterItemModel>>>
    {
        private readonly ICategoryDao categoryDao;
        public GetFilterItemsQueryHandler(ICategoryDao categoryDao)
        {
            this.categoryDao = categoryDao;
        }
        public async Task<Result<List<FilterItemModel>>> Handle(GetFilterItemsQueryRequest request, CancellationToken cancellationToken)
        {
            Category? category = await categoryDao.GetByName(request.categoryName);
            if (category == null)
            {
                return new("Category not found");
            }
            //CategoryPropertyKey[]? categoryProps = await _context
            //     .CategoryPropertyKeys
            //     .Where(cp => cp.CategoryId == categoryId)
            //     .ToArrayAsync();
            //var filterItems = await _context.ProductProperties
            //    .Join(_context.CategoryPropertyKeys,
            //    pprops => pprops.Key,
            //    catprops => catprops.Name,
            //    (pprops, catprops) => new { pprops, catprops })
            //    .Where(x => x.catprops.CategoryId == categoryId && x.catprops.IsFilter)
            //    .GroupBy(x => new { x.pprops.Key, x.pprops.Value })
            //    .Select(g => new FilterItemModel
            //    {
            //        Key = g.Key.Key,
            //        Value = g.Key.Value,
            //        Count = g.Count()
            //    }).ToListAsync();
            //if (filterItems != null && filterItems.Count != 0)
            //{
            //    return filterItems;
            //}
            List<FilterItemModel>? filterItems = await categoryDao.GetFilterItems(category.Id);
            if (filterItems != null)
            {
                return new(filterItems);
            }
            return new("Filter items not found");
        }
    }
}
