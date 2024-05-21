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
    public class GetReviewTagsQueryHandler : IRequestHandler<GetReviewTagsQueryRequest, Result<List<ReviewTagProfile>>>
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public GetReviewTagsQueryHandler(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public async Task<Result<List<ReviewTagProfile>>> Handle(GetReviewTagsQueryRequest request, CancellationToken cancellationToken)
        {
            List<ReviewTag>? reviewTags = await _dataContext.ReviewTags.AsNoTracking().ToListAsync();
            if (reviewTags != null && reviewTags.Count != 0)
            {
                return new(_mapper.Map<List<ReviewTagProfile>>(reviewTags));
            }
            return new("Not found");
        }
    }
}
