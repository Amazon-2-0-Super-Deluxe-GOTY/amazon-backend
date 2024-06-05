using amazon_backend.CQRS.Queries.Request.CategoryRequests;
using amazon_backend.Data;
using amazon_backend.Models;
using amazon_backend.Profiles.CategoryProfiles;
using amazon_backend.Profiles.ReviewProfiles;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
namespace amazon_backend.CQRS.Handlers.QueryHandlers.CategoryQueryHandlers
{
    public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQueryRequest, Result<List<CategoryViewProfile>>>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        public GetCategoryQueryHandler(IMapper mapper, DataContext dataContext)
        {
            _mapper = mapper;
            _dataContext = dataContext;
        }

        public async Task<Result<List<CategoryViewProfile>>> Handle(GetCategoryQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _dataContext.Categories
                .Where(q => q.IsDeleted == false) 
                .Include(r => r.ParentCategoryName)
                .Include(r => r.ParentCategory)
                .Include(r => r.ParentCategoryId)
                .Include(r => r.CategoryPropertyKeys)
                .Include(r => r.Image)
                .AsQueryable();

            if (request.categorytId != 0)
            {
                query = query.Where(r => r.Name  == request.categoryName);
            }
            query = request.byAsc.GetValueOrDefault()
            ? query.OrderBy(r => r.IsVisible)
                : query.OrderByDescending(r => r.IsVisible);

            var categories = await query
                .Skip(request.pageSize * (request.pageIndex - 1))
                .Take(request.pageSize)
                .ToListAsync(cancellationToken);

            var categoryProfile = _mapper.Map<List<CategoryViewProfile>>(categories);

            int pagesCount = (int)Math.Ceiling(query.Count() / (double)request.pageSize);
            if (categoryProfile != null && categoryProfile.Count != 0)
            {
                return new(categoryProfile, pagesCount);
            }
            return new("Reviews not found");
        }

     
    }
}
