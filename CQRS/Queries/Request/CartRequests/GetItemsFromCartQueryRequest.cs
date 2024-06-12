using amazon_backend.Models;
using amazon_backend.Profiles.CartProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.CartRequests
{
    public class GetItemsFromCartQueryRequest : IRequest<Result<CartProfile>>
    {
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
    }
    public class GetItemsFromCartValidator : AbstractValidator<GetItemsFromCartQueryRequest>
    {
        public GetItemsFromCartValidator()
        {
            RuleFor(x => x.pageSize).NotEmpty().GreaterThan(0);
            RuleFor(x => x.pageIndex).NotEmpty().GreaterThan(0);
        }
    }
}
