using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.WishListRequests
{
    public class GetWishListQueryRequest : IRequest<Result<List<ProductCardProfile>>>
    {
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public string? searchQuery { get; set; }
    }
    public class GetWishListValidator : AbstractValidator<GetWishListQueryRequest>
    {
        public GetWishListValidator()
        {
            RuleFor(x => x.pageSize).NotEmpty().GreaterThan(0);
            RuleFor(x => x.pageIndex).NotEmpty().GreaterThan(0);
        }
    }
}
