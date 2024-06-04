using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.ProductRequests
{
    public class GetProductBySlugQueryRequest : IRequest<Result<ProductViewProfile>>
    {
        public string productSlug { get; set; }
    }
    public class GetProductBySlugValidator: AbstractValidator<GetProductBySlugQueryRequest>
    {
        public GetProductBySlugValidator()
        {
            RuleFor(x => x.productSlug).NotEmpty().WithMessage("Slug cannot be empty");
        }
    }
}
