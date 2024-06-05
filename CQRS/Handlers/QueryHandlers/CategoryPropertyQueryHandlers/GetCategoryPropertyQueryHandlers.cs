using Amazon.S3;
using amazon_backend.CQRS.Commands.ReviewRequests;
using amazon_backend.CQRS.Queries.Request.CategoryPropertyKeyRequests;
using amazon_backend.CQRS.Queries.Request.CategoryRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Data.Model;
using amazon_backend.Models;
using amazon_backend.Profiles.CategoryProfiles;

using amazon_backend.Services.AWSS3;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.CategoryPropertyQueryHandlers
{
    public class GetCategoryPropertyQueryHandlers : IRequestHandler<GetCategoryPropertyKeyRequest, Result<List<CategoryPropertyKeyProfile>>>
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public GetCategoryPropertyQueryHandlers(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

      

        public async Task<Result<List<CategoryPropertyKeyProfile>>> Handle(GetCategoryPropertyKeyRequest request, CancellationToken cancellationToken)
        {
            List<CategoryPropertyKey>? propertyKeys = await _dataContext.CategoryPropertyKeys.AsNoTracking().ToListAsync();
            if (propertyKeys != null && propertyKeys.Count != 0)
            {
                return new(_mapper.Map<List<CategoryPropertyKeyProfile>>(propertyKeys));
            }
            return new("Not found");
        }
    }
}
