using amazon_backend.Data.Model;
using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request
{
    public class GetFilterItemsQueryRequest:IRequest<Result<List<FilterItemModel>>>
    {
        public string categoryName { get; set; }
    }
    public class GetFilterItemsValidator : AbstractValidator<GetFilterItemsQueryRequest>
    {
        public GetFilterItemsValidator()
        {
            RuleFor(x => x.categoryName).NotNull().NotEmpty();
        }
    }
}
