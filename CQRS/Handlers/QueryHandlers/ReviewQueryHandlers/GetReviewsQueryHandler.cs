using amazon_backend.CQRS.Queries.Request.ReviewsRequests;
using amazon_backend.Data;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class GetReviewsQueryHandler : IRequestHandler<GetReviewsQueryRequest, Result<List<ReviewProfile>>>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TokenService _tokenService;

        public GetReviewsQueryHandler(IMapper mapper, DataContext dataContext, IHttpContextAccessor httpContextAccessor, TokenService tokenService)
        {
            _mapper = mapper;
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
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
            else if (!string.IsNullOrEmpty(request.userId))
            {
                query = query.Where(r => r.UserId == Guid.Parse(request.userId));
            }
            if (request.rating.HasValue)
            {
                query = query.Where(r => r.Mark == request.rating.Value);
            }

            if (!string.IsNullOrEmpty(request.orderBy))
            {
                switch (request.orderBy)
                {
                    case "asc":
                        query = query.OrderBy(q => q.CreatedAt);
                        break;
                    case "desc":
                        query = query.OrderByDescending(q => q.CreatedAt);
                        break;
                    case "like":
                        query = query.OrderByDescending(q => q.ReviewLikes!.Count());
                        break;
                    default:
                        query = query.OrderByDescending(q => q.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(q => q.CreatedAt);
            }

            var reviews = await query
                .Skip(request.pageSize * (request.pageIndex - 1))
                .Take(request.pageSize)
                .ToListAsync(cancellationToken);


            if (reviews != null && reviews.Count != 0)
            {
                var reviewsProfiles = _mapper.Map<List<ReviewProfile>>(reviews);
                string? token = null;
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    token = httpContext.Request.Cookies["jwt"];
                }
                if (token != null)
                {
                    var decodeResult = await _tokenService.DecodeToken("Bearer " + token, false);
                    await Console.Out.WriteLineAsync($"{decodeResult.message}");
                    if (decodeResult.isSuccess)
                    {
                        var user = decodeResult.data;
                        foreach (var item in reviewsProfiles)
                        {
                            item.CurrentUserLiked = reviews
                                .FirstOrDefault(r => r.Id == item.Id)?
                                .ReviewLikes.Any(like => like.UserId == user.Id) ?? false;
                        }
                    }
                }
                int pagesCount = (int)Math.Ceiling(query.Count() / (double)request.pageSize);
                return new(reviewsProfiles, pagesCount) { statusCode = 200 };
            }
            return new("Reviews not found") { statusCode = 404 };
        }
    }
}
