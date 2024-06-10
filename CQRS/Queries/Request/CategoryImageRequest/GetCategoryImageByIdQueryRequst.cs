using amazon_backend.CQRS.Queries.Request.ProductImageRequests;
using amazon_backend.Models;
using amazon_backend.Profiles.CategoryProfiles;
using amazon_backend.Profiles.ProductImageProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.CategoryImageRequest
{
    public class GetCategoryImageByIdQueryRequst : IRequest<Result<CategoryImageProfile>>
    {
        public string categoryImageId { get; set; }
    }
    public class GetCategoryImageByIdValidator : AbstractValidator<GetCategoryImageByIdQueryRequst>
    {
        public GetCategoryImageByIdValidator()
        {
            RuleFor(x => x.categoryImageId)
               .Must(x => Guid.TryParse(x, out var result) == true)
               .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
