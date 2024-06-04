using amazon_backend.CQRS.Queries.Request.ProductImageRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ProductImageProfiles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewImageQueryHandlers.QueryHandlers
{
    public class GetProductImageByIdQueryHandler : IRequestHandler<GetProductImageByIdQueryRequest, Result<ProductImageProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public GetProductImageByIdQueryHandler(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<Result<ProductImageProfile>> Handle(GetProductImageByIdQueryRequest request, CancellationToken cancellationToken)
        {
            ProductImage? productImage = await _dataContext.ProductImages
                .AsNoTracking()
                .FirstOrDefaultAsync(ri => ri.Id == Guid.Parse(request.productImageId));
            if(productImage == null)
            {
                return new("Image not found") { statusCode = 404 };
            }
            return new(_mapper.Map<ProductImageProfile>(productImage)) { statusCode = 200, message="Ok" };
        }
    }
}
