using amazon_backend.Models;
using amazon_backend.Profiles.ReviewImageProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.ReviewImageRequests
{
    public class GetReviewImageByIdQueryRequest:IRequest<Result<ReviewImageProfile>>
    {
        public string reviewImageId { get; set; }
    }
    public class GetReviewImageByIdValidator : AbstractValidator<GetReviewImageByIdQueryRequest>
    {
        public GetReviewImageByIdValidator()
        {
            RuleFor(x => x.reviewImageId)
               .Must(x => Guid.TryParse(x, out var result) == true)
               .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
