using amazon_backend.Models;
using amazon_backend.Profiles.ReviewImageProfiles;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace amazon_backend.CQRS.Commands.ReviewImageRequests
{

    public class CreateReviewImageCommandRequest:IRequest<Result<ReviewImageProfile>>
    {
        public IFormFile reviewImage { get; set; }
    }

    public class CreateReviewImageValidator:AbstractValidator<CreateReviewImageCommandRequest>
    {
        public CreateReviewImageValidator()
        {
            RuleFor(x => x.reviewImage.Length).LessThanOrEqualTo(5 * 1024 * 1024)
                    .WithMessage("File size must be less than or equal to 5 MB");
            RuleFor(x => x.reviewImage.ContentType).Must(contentType =>
                contentType == "image/jpeg" || contentType == "image/png")
                .WithMessage("Only JPEG and PNG files are allowed");
        }
    }

}
