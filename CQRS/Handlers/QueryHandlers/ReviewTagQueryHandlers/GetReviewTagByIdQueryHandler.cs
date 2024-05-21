using amazon_backend.CQRS.Queries.Request.ReviewTagRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewTagQueryHandlers
{
    public class GetReviewTagByIdQueryHandler : IRequestHandler<GetReviewTagByIdQueryRequest, Result<ReviewTagProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public GetReviewTagByIdQueryHandler(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public async Task<Result<ReviewTagProfile>> Handle(GetReviewTagByIdQueryRequest request, CancellationToken cancellationToken)
        {
            ReviewTag? reviewTag = await _dataContext
                .ReviewTags
                .Where(rt => rt.Id == Guid.Parse(request.reviewTagId))
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (reviewTag != null)
            {
                return new(_mapper.Map<ReviewTagProfile>(reviewTag));
            }
            return new("Review tag not found");
        }
    }
}
