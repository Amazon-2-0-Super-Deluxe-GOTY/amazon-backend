using amazon_backend.CQRS.Queries.Request.ReviewsRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class GetReviewsQueryHandler : IRequestHandler<GetReviewsQueryRequest, Result<List<ReviewProfile>>>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        public GetReviewsQueryHandler(IMapper mapper, DataContext dataContext)
        {
            _mapper = mapper;
            _dataContext = dataContext;
        }
        public async Task<Result<List<ReviewProfile>>> Handle(GetReviewsQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _dataContext.Reviews
                .Where(q=>q.DeletedAt==null)
                .Include(r=>r.ReviewTags)
                .Include(r=>r.ReviewImages)
                .AsQueryable();
            if (!string.IsNullOrEmpty(request.userId) && !string.IsNullOrEmpty(request.productId))
            {
                query = query
                    .Where(r => r.UserId == Guid.Parse(request.userId) && r.ProductId == Guid.Parse(request.productId));
            }
            else if (!string.IsNullOrEmpty(request.productId))
            {
                query = query.Where(r => r.ProductId == Guid.Parse(request.productId));
            }
            else if (!string.IsNullOrEmpty(request.userId))
            {
                query = query.Where(r => r.UserId == Guid.Parse(request.userId));
            }
            if (request.rating.HasValue)
            {
                query = query.Where(r => r.Mark == request.rating.Value);
            }

            query = request.byAsc.GetValueOrDefault()
            ? query.OrderBy(r => r.CreatedAt)
                : query.OrderByDescending(r => r.CreatedAt);

            var reviews = await query
                .Skip(request.pageSize * (request.pageIndex - 1))
                .Take(request.pageSize)
                .ToListAsync(cancellationToken);
            var reviewsProfiles = _mapper.Map<List<ReviewProfile>>(reviews);
            int pagesCount = (int)Math.Ceiling(query.Count() / (double)request.pageSize);
            if (reviewsProfiles != null && reviewsProfiles.Count != 0)
            {
                return new(reviewsProfiles, pagesCount);
            }
            return new("Reviews not found");
        }
    }
}
