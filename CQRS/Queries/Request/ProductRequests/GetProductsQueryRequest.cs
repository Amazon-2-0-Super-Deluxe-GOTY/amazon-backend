using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.ProductRequests
{
    public class GetProductsQueryRequest : IRequest<Result<List<ProductCardProfile>>>
    {
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public bool? discount { get; set; }
        public string? categoryName { get; set; }
        public int? rating { get; set; }
        public double? minPrice { get; set; }
        public double? maxPrice { get; set; }
        public string? orderBy { get; set; }
        public bool? byAsc { get; set; }
        public IDictionary<string,string>? filters { get; set; }
    }
    public class GetProductsByCategoryValidator : AbstractValidator<GetProductsQueryRequest>
    {
        public GetProductsByCategoryValidator()
        {
            RuleFor(x => x.pageSize).NotEmpty().GreaterThan(0);
            RuleFor(x => x.pageIndex).NotEmpty().GreaterThan(0);
            RuleFor(x => x.rating)
                .GreaterThan(0)
                .LessThan(6)
                .When(x => x.rating.HasValue);
        }
    }
}
