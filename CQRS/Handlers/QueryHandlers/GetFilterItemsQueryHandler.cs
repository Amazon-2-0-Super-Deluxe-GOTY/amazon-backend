using amazon_backend.CQRS.Queries.Request;
using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using amazon_backend.Data.Model;
using amazon_backend.Models;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers
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
            List<FilterItemModel>? filterItems = await categoryDao.GetFilterItems(category.Id);
            if (filterItems != null)
            {
                return new(filterItems);
            }
            return new("Filter items not found");
        }
    }
}
