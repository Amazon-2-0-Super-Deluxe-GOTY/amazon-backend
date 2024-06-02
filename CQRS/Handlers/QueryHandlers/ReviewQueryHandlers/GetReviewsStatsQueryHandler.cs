using amazon_backend.CQRS.Queries.Request.ReviewsRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Profiles.ReviewProfiles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewQueryHandlers
{
    public class GetReviewsStatsQueryHandler : IRequestHandler<GetReviewsStatsQueryRequest, Result<object>>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        public GetReviewsStatsQueryHandler(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<Result<object>> Handle(GetReviewsStatsQueryRequest request, CancellationToken cancellationToken)
        {
            var productId = Guid.Parse(request.productId);
            Product? product = await _dataContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                return new("Product not found") { statusCode = 404 };
            }

            List<Review> reviews = await _dataContext
                .Reviews
                .Include(r => r.ReviewTags)
                .AsNoTracking()
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (reviews == null || reviews.Count == 0)
            {
                return new("Reviews not found") { statusCode = 404 };
            }

            var uniqueTags = reviews
                .SelectMany(r => r.ReviewTags!)
                .GroupBy(rt => rt.Name)
                .Select(g => _mapper.Map<ReviewTagProfile>(g.First()))
                .ToList();

            var totalRates = reviews.Count;

            var rates = reviews
                .GroupBy(pr => pr.Mark)
                .Select(group => new RatingStat
                {
                    mark = group.Key,
                    percent = (int)(group.Count() * 100.0 / totalRates)
                })
                .OrderByDescending(r => r.mark)
                .ToList();

            double generalRate = reviews.Average(r => r.Mark);

            var result = new
            {
                generalRate = Math.Round(generalRate, 1),
                reviewsQuantity = totalRates,
                ratingStats = rates,
                tags = uniqueTags
            };

            return new(result) { message = "Ok", statusCode = 200 };
        }
    }
}
