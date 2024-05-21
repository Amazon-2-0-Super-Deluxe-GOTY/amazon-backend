using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.ProductRequests
{
    public class GetProductByIdQueryRequest : IRequest<Result<ProductViewProfile>>
    {
        public string productId { get; set; }
    }
    public class GetProductByIdValidator : AbstractValidator<GetProductByIdQueryRequest>
    {
        public GetProductByIdValidator()
        {
            RuleFor(x => x.productId).NotEmpty();
            RuleFor(x => x.productId).Must(x => Guid.TryParse(x, out var result) == true).WithMessage("Incorrect {PropertyName} format");
        }
    }
}
