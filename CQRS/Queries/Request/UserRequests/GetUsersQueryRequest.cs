using amazon_backend.Models;
using amazon_backend.Profiles.UserProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.UserRequests
{
    public class GetUsersQueryRequest:IRequest<Result<List<ClientProfile>>>
    {
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public string? searchQuery { get; set; }
        public string? orderBy { get; set; }
    }
    public class GetUsersValidator : AbstractValidator<GetUsersQueryRequest>
    {
        public GetUsersValidator()
        {
            RuleFor(x => x.pageSize).NotEmpty().GreaterThan(0);
            RuleFor(x => x.pageIndex).NotEmpty().GreaterThan(0);
        }
    }
}
