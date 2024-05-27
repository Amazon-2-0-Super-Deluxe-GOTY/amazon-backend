using amazon_backend.Models;
using amazon_backend.Profiles.UserProfiles;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.UserRequests
{
    public class GetUserProfileQueryRequest:IRequest<Result<ClientProfile>>
    {
    }
}
