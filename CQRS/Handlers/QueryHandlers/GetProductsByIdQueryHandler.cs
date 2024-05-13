using amazon_backend.CQRS.Queries.Request;
using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using AutoMapper;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers
{
    public class GetProductsByIdQueryHandler : IRequestHandler<GetProductsByCategoryQueryRequest, Result<List<ProductCardProfile>>>
    {
        private readonly IProductDao _productDao;
        private readonly ICategoryDao _categoryDao;
        private readonly IMapper mapper;

        public GetProductsByIdQueryHandler(IProductDao productDao, ICategoryDao categoryDao, IMapper mapper)
        {
            _productDao = productDao;
            _categoryDao = categoryDao;
            this.mapper = mapper;
        }

        public async Task<Result<List<ProductCardProfile>>> Handle(GetProductsByCategoryQueryRequest request, CancellationToken cancellationToken)
        {
            Category? _category = await _categoryDao.GetByName(request.category);
            if (_category is null) return new("Category not found");

            Product[]? products = await _productDao
                .GetProductsByCategoryLimit(_category.Id, request.pageSize, request.pageIndex);

            if (products != null)
            {
                if (products.Length != 0)
                {
                    List<ProductCardProfile> productCards = mapper.Map<List<ProductCardProfile>>(products);
                    return new(productCards);
                }
            }
            return new("Products not found");
        }
    }
}
