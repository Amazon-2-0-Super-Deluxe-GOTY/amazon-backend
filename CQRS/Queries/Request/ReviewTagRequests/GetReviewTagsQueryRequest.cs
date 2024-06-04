using amazon_backend.Models;
using amazon_backend.Profiles.ReviewTagProfiles;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.ReviewTagRequests
{
    public class GetReviewTagsQueryRequest:IRequest<Result<List<ReviewTagProfile>>>
    {
    }
}
