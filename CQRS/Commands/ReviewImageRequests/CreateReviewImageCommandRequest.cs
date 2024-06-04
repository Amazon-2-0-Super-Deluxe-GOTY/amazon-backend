using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewImageProfiles;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace amazon_backend.CQRS.Commands.ReviewImageRequests
{

    public class CreateReviewImageCommandRequest:IRequest<Result<List<ReviewImageProfile>>>
    {
        public List<IFormFile> reviewImages { get; set; }
    }

    public class CreateReviewImageValidator:AbstractValidator<CreateReviewImageCommandRequest>
    {
        public CreateReviewImageValidator()
        {
            RuleFor(x => x.reviewImages)
                .Must(reviewImages => reviewImages.Count >= 1 && reviewImages.Count <= 10)
                .WithMessage("You must add at least 1 photo. Maximum 10 photos");

            RuleForEach(x => x.reviewImages).ChildRules(reviewImage =>
            {
                reviewImage.RuleFor(reviewImage => reviewImage.Length).LessThanOrEqualTo(5 * 1024 * 1024)
                    .WithMessage("File size must be less than or equal to 5 MB");

                reviewImage.RuleFor(reviewImage => reviewImage.ContentType).Must(contentType =>
                 contentType == "image/jpeg" || contentType == "image/png")
                 .WithMessage("Only JPEG and PNG files are allowed");
            });
        }
    }

}
