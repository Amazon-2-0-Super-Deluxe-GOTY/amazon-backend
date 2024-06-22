using amazon_backend.Models;
using amazon_backend.Profiles.AboutProductProfiles;
using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Profiles.ProductPropertyProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.ProductCommands
{
    public class UpdateProductCommandRequest : IRequest<Result<ProductViewProfile>>
    {
        public string productId { get; set; }
        public string? name { get; set; }
        public string? code { get; set; }
        public int? categoryId { get; set; }
        public double? price { get; set; }
        public int? discount { get; set; }
        public int? quantity { get; set; }
        public List<string>? images { get; set; }
        public List<ProductPropFormProfile>? productDetails { get; set; }
        public List<AboutProductFormProfile>? aboutProduct { get; set; }
    }
    public class UpdateProductValidator : AbstractValidator<UpdateProductCommandRequest>
    {
        private readonly int imagesMaxCount = 10;
        private readonly int propTextMaxCharCount = 30;
        private readonly int aboutProductTextMaxCharCount = 500;

        public UpdateProductValidator()
        {
            // productId validate

            RuleFor(x => x.productId)
                .NotEmpty().WithMessage("Product id cannot be empty")
                .Must(productId => Guid.TryParse(productId, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format");

            // code validate
            RuleFor(x => x.code).Must((code) =>
            {
                foreach (var item in code)
                {
                    if (!int.TryParse(item.ToString(), out var _))
                    {
                        return false;
                    }
                }
                return true;
            }).WithMessage("Barcode is invalid")
            .Length(13)
            .WithMessage("Barcode length must be 13 characters")
            .When(x => x.code != null);

            // category validate
            RuleFor(x => x.categoryId)
                .GreaterThan(0)
                .When(x=>x.categoryId.HasValue);

            // price validate
            RuleFor(x => x.price)
                .GreaterThan(0)
                .WithMessage("Price must be a positive number")
                .When(x => x.price.HasValue);

            // discount validate
            RuleFor(x => x.discount).GreaterThan(-1).LessThan(101).WithMessage("Discount must be a positive number. Only numbers from 0 to 100 are allowed")
                .When(x => x.discount.HasValue);

            // quantity validate
            RuleFor(x => x.quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be a positive number")
                .When(x => x.quantity.HasValue);

            // images validate
            RuleFor(x => x.images)
                .Must(x => x.Count <= imagesMaxCount)
                .WithMessage($"There should be a maximum of {imagesMaxCount} photos")
                .Must(images => images.Count >= 1)
                .WithMessage("Add at least one photo")
                .When(x => x.images != null);


            RuleForEach(x => x.images).ChildRules(images =>
            {
                images.RuleFor(x => x)
                .Must(id => Guid.TryParse(id, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format"); ;
            }).When(x => x.images != null);

            // productDetails validate
            RuleFor(x => x.productDetails)
                .Must(productDetails => productDetails.Count >= 1)
                .WithMessage("You must describe at least 1 option")
                .When(x => x.productDetails != null);

            RuleForEach(x => x.productDetails).ChildRules(productDetails =>
            {
                productDetails.RuleFor(x => x.name)
                .NotEmpty()
                .WithMessage("Name cannot be empty");

                productDetails.RuleFor(x => x.text)
                .NotEmpty()
                .WithMessage("Text cannot be empty")
                .MaximumLength(propTextMaxCharCount)
                .WithMessage($"Text should be no more than {propTextMaxCharCount} characters");
            }).When(x => x.productDetails != null);

            // aboutProduct validate

            RuleForEach(x => x.aboutProduct).ChildRules(aboutProduct =>
            {
                aboutProduct.RuleFor(x => x.name).NotEmpty()
                .WithMessage("Name cannot be empty");

                aboutProduct.RuleFor(x => x.text).NotEmpty()
                .WithMessage("Text cannot be empty")
                .MaximumLength(aboutProductTextMaxCharCount)
                .WithMessage($"Text should be no more than {aboutProductTextMaxCharCount} characters");
            }).When(x => x.aboutProduct != null);
        }
    }
}
