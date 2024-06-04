using amazon_backend.Models;
using amazon_backend.Profiles.ProductImageProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.ProductImageRequests
{
    public class GetProductImageByIdQueryRequest:IRequest<Result<ProductImageProfile>>
    {
        public string productImageId { get; set; }
    }
    public class GetProductImageByIdValidator : AbstractValidator<GetProductImageByIdQueryRequest>
    {
        public GetProductImageByIdValidator()
        {
            RuleFor(x => x.productImageId)
               .Must(x => Guid.TryParse(x, out var result) == true)
               .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
