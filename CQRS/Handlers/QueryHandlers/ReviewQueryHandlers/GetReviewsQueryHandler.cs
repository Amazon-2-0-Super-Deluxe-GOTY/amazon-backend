using amazon_backend.CQRS.Queries.Request.ReviewsRequests;
using amazon_backend.Data;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using AutoMapper;
using MediatR;
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
                .Where(q => q.DeletedAt == null)
                .Include(r => r.ReviewTags)
                .Include(r => r.ReviewImages)
                .Include(r => r.User)
                .Include(r => r.ReviewLikes)
                .AsSplitQuery()
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
            
            if (reviews != null && reviews.Count != 0)
            {
                var reviewsProfiles = _mapper.Map<List<ReviewProfile>>(reviews);
                if (request.userId != null) {
                    var userId = Guid.Parse(request.userId);
                    foreach (var item in reviewsProfiles)
                    {
                        item.CurrentUserLiked = reviews
                            .FirstOrDefault(r => r.Id == item.Id)?
                            .ReviewLikes.Any(like => like.UserId == userId) ?? false;
                    }
                }
                int pagesCount = (int)Math.Ceiling(query.Count() / (double)request.pageSize);
                return new(reviewsProfiles, pagesCount) { statusCode = 200 };
            }
            return new("Reviews not found") { statusCode = 404 };
        }
    }
}
