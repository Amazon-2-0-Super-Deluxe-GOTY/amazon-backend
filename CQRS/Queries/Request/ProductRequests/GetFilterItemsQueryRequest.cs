using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.ProductRequests
{
    public class GetFilterItemsQueryRequest : IRequest<Result<object>>
    {
        public int categoryId { get; set; }
    }
    public class GetFilterItemsValidator : AbstractValidator<GetFilterItemsQueryRequest>
    {
        public GetFilterItemsValidator()
        {
            RuleFor(x => x.categoryId).GreaterThan(0).NotEmpty();
        }
    }
}
