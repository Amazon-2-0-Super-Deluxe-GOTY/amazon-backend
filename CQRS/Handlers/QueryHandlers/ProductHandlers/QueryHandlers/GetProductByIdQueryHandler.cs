using amazon_backend.CQRS.Queries.Request.ProductRequests;
using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using AutoMapper;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ProductHandlers.QueryHandlers
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQueryRequest, Result<ProductViewProfile>>
    {
        private readonly IProductDao productDao;
        private readonly IMapper mapper;

        public GetProductByIdQueryHandler(IProductDao productDao, IMapper mapper)
        {
            this.productDao = productDao;
            this.mapper = mapper;
        }

        public async Task<Result<ProductViewProfile>> Handle(GetProductByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var productId = Guid.Parse(request.productId);
            Product? product = await productDao.GetByIdAsync(productId);
            if (product == null)
            {
                return new("Product not found") { statusCode = 404 };
            }
            return new(mapper.Map<ProductViewProfile>(product)) { statusCode = 200 };
        }
    }
}
