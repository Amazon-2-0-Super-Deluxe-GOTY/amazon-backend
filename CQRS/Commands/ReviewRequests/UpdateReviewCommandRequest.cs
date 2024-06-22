using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.ReviewRequests
{
    public class UpdateReviewCommandRequest : IRequest<Result<ReviewProfile>>
    {
        public string reviewId { get; set; }
        public int? rating { get; set; }
        public string? title { get; set; }
        public string? text { get; set; }
        public List<string>? reviewImagesIds { get; set; }
        public List<string>? reviewTagsIds { get; set; }
    }
    public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommandRequest>
    {
        public UpdateReviewCommandValidator()
        {
            RuleFor(x => x.reviewId)
                .Must(x => Guid.TryParse(x, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format");

            RuleFor(x => x.rating).GreaterThan(0).LessThan(6).When(x=>x.rating.HasValue);

            RuleFor(x => x.reviewImagesIds).Must(x => x.Count > 0 && x.Count <= 10)
                .When(x => x.reviewImagesIds != null && x.reviewImagesIds.Count != 0)
                .WithMessage("There should be a maximum of 10 images");

            RuleForEach(x => x.reviewImagesIds).ChildRules(images =>
            {
                images.RuleFor(x => x).Must(id => Guid.TryParse(id, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format"); ;
            });

            RuleFor(x => x.title).MaximumLength(250).When(x => !string.IsNullOrEmpty(x.title));

            RuleFor(x => x.text).MaximumLength(1000).When(x=>!string.IsNullOrEmpty(x.text));

            RuleForEach(x => x.reviewTagsIds).ChildRules(tags =>
            {
                tags.RuleFor(x => x).Must(id => Guid.TryParse(id, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format"); ;
            });
        }
    }
}
