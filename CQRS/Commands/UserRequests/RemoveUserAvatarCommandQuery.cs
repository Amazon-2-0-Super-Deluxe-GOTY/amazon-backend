using amazon_backend.Models;
using MediatR;

namespace amazon_backend.CQRS.Commands.UserRequests
{
    public class RemoveUserAvatarCommandQuery:IRequest<Result<string>>
    {
    }
}
