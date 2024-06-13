using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.ReviewsRequests
{
    public class GetReviewByIdQueryRequest:IRequest<Result<ReviewProfile>>
    {
        public string reviewId { get; set; }
    }
    public class GetReviewByIdValidator: AbstractValidator<GetReviewByIdQueryRequest>
    {
        public GetReviewByIdValidator()
        {
            RuleFor(x => x.reviewId)
                .NotEmpty()
                .NotNull()
                .Must(x => Guid.TryParse(x, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
