using amazon_backend.Models;
using amazon_backend.Profiles.OrderProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.OrderRequests
{
    public class GetOrdersQueryRequest : IRequest<Result<List<OrderProfile>>>
    {
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
    }
    public class GetOrdersValidator : AbstractValidator<GetOrdersQueryRequest>
    {
        public GetOrdersValidator()
        {
            RuleFor(x => x.pageIndex).GreaterThan(0);
            RuleFor(x => x.pageSize).GreaterThan(0);
        }
    }
}
