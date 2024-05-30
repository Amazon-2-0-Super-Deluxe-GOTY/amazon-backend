using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace amazon_backend.CQRS.Commands.ReviewRequests
{
    public class CreateReviewCommandRequest:IRequest<Result<ReviewProfile>>
    {
        public string productId { get; set; }
        public int rating { get; set; }
        public string? text { get; set; }
        public List<string>? reviewImagesIds { get; set; }
        public List<string>? reviewTagsIds { get; set; }
    }
    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommandRequest>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(x => x.productId)
                .Must(x => Guid.TryParse(x, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format");

            RuleFor(x => x.rating).NotEmpty().GreaterThan(0).LessThan(6);

            RuleFor(x => x.text).MaximumLength(250);

            RuleFor(x => x.reviewImagesIds).Must(x => x.Count > 0 && x.Count <= 10)
                .When(x => x.reviewImagesIds != null && x.reviewImagesIds.Count != 0)
                .WithMessage("There should be a maximum of 10 images");

            RuleForEach(x => x.reviewImagesIds).ChildRules(images =>
            {
                images.RuleFor(x => x).Must(id => Guid.TryParse(id, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format"); ;
            });

            RuleForEach(x => x.reviewTagsIds).ChildRules(tags =>
            {
                tags.RuleFor(x => x).Must(id => Guid.TryParse(id, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format"); ;
            });
        }
    }
}
