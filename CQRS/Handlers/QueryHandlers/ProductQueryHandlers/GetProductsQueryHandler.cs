using amazon_backend.CQRS.Queries.Request.ProductRequests;
using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using AutoMapper;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ProductQueryHandlers
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQueryRequest, Result<List<ProductCardProfile>>>
    {
        private readonly IProductDao _productDao;
        private readonly ICategoryDao _categoryDao;
        private readonly IMapper mapper;

        public GetProductsQueryHandler(IProductDao productDao, ICategoryDao categoryDao, IMapper mapper)
        {
            _productDao = productDao;
            _categoryDao = categoryDao;
            this.mapper = mapper;
        }

        public async Task<Result<List<ProductCardProfile>>> Handle(GetProductsQueryRequest request, CancellationToken cancellationToken)
        {
            Category? _category = await _categoryDao.GetByName(request.category);
            if (_category is null) return new("Category not found");


            List<Product>? products = await _productDao
                .GetProductsByCategory(_category.Id);

            if (products != null)
            {
                if (products.Count != 0)
                {
                    int pagesCount = (int)Math.Ceiling(products.Count / (double)request.pageSize);
                    List<ProductCardProfile> productCards = mapper.Map<List<ProductCardProfile>>(products.Skip((request.pageIndex - 1) * request.pageSize)
                .Take(request.pageSize));
                    return new(productCards, pagesCount);
                }
            }
            return new("Products not found");
        }
    }
}
