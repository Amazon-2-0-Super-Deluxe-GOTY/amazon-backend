using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request
{
    public class GetProductsQueryRequest:IRequest<Result<List<ProductCardProfile>>>
    {
        public string category { get; set; }
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
    }
    public class GetProductsByCategoryValidator:AbstractValidator<GetProductsQueryRequest>
    {
        public GetProductsByCategoryValidator()
        {
            RuleFor(x => x.category).NotNull().NotEmpty();
            RuleFor(x => x.pageSize).NotEmpty().GreaterThan(0);
            RuleFor(x => x.pageIndex).NotEmpty().GreaterThan(0);
        }
    }
}
