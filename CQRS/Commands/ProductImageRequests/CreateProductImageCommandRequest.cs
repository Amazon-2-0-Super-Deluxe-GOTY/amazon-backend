using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ProductImageProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.ReviewImageRequests
{
    public class CreateProductImageCommandRequest : IRequest<Result<List<ProductImageProfile>>>
    {
        public List<IFormFile> productImages { get; set; }
    }

    public class CreateProductImageValidator : AbstractValidator<CreateProductImageCommandRequest>
    {
        public CreateProductImageValidator()
        {
            RuleFor(x => x.productImages)
                 .Must(productImages => productImages.Count >= 1 && productImages.Count <= 10)
                 .WithMessage("You must add at least 1 photo. Maximum 10 photos");

            RuleForEach(x => x.productImages).ChildRules(productImage =>
            {
                productImage.RuleFor(productImage => productImage.Length).LessThanOrEqualTo(5 * 1024 * 1024)
                    .WithMessage("File size must be less than or equal to 5 MB");

                productImage.RuleFor(productImage => productImage.ContentType).Must(contentType =>
                 contentType == "image/jpeg" || contentType == "image/png")
                 .WithMessage("Only JPEG and PNG files are allowed");
            });
        }
    }

}
