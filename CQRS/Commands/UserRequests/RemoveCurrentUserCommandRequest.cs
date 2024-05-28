using amazon_backend.Models;
using MediatR;

namespace amazon_backend.CQRS.Commands.UserRequests
{
    public class RemoveCurrentUserCommandRequest:IRequest<Result<string>>
    {
    }
}
