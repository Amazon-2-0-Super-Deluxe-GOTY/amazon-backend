using amazon_backend.CQRS.Queries.Request.ReviewImageRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewImageQueryHandlers.QueryHandlers
{
    public class GetReviewImageByIdQueryHandler : IRequestHandler<GetReviewImageByIdQueryRequest, Result<ReviewImageProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public GetReviewImageByIdQueryHandler(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<Result<ReviewImageProfile>> Handle(GetReviewImageByIdQueryRequest request, CancellationToken cancellationToken)
        {
            ReviewImage? reviewImage = await _dataContext.ReviewImages
                .AsNoTracking()
                .FirstOrDefaultAsync(ri => ri.Id == Guid.Parse(request.reviewImageId));
            if(reviewImage == null)
            {
                return new("Image not found") { statusCode = 404 };
            }
            return new(_mapper.Map<ReviewImageProfile>(reviewImage)) { statusCode = 200, message="Ok" };
        }
    }
}
