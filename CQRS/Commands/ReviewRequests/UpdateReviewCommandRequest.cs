using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.ReviewRequests
{
    public class UpdateReviewCommandRequest : IRequest<Result<Guid>>
    {
        //public string userToken { get; set; }
        public string reviewId { get; set; }
        public int rating { get; set; }
        public string? text { get; set; }
        public List<IFormFile>? reviewImages { get; set; }
        public List<string>? reviewTagsIds { get; set; }
    }
    public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommandRequest>
    {
        public UpdateReviewCommandValidator()
        {
            RuleFor(x => x.reviewId)
                .Must(x => Guid.TryParse(x, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format");
            RuleFor(x => x.rating).NotEmpty().GreaterThan(0).LessThan(6);
            RuleFor(x => x.text).MaximumLength(250);
            RuleFor(x => x.reviewImages).Must(x => x.Count > 0 && x.Count <= 10)
                .When(x => x.reviewImages != null && x.reviewImages.Count != 0)
                .WithMessage("There should be a maximum of 10 images");
            RuleForEach(x => x.reviewImages).ChildRules(images =>
            {
                images.RuleFor(image => image.Length).LessThanOrEqualTo(2 * 1024 * 1024)
                    .WithMessage("File size must be less than or equal to 2 MB");
                images.RuleFor(image => image.ContentType).Must(contentType =>
                    contentType == "image/jpeg" || contentType == "image/png")
                    .WithMessage("Only JPEG and PNG files are allowed");
            });
            RuleForEach(x => x.reviewTagsIds).ChildRules(tags =>
            {
                tags.RuleFor(x => x).Must(id => Guid.TryParse(id, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format"); ;
            });
        }
    }
}
