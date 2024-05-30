using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.ReviewTagRequests
{
    public class GetReviewTagByIdQueryRequest:IRequest<Result<ReviewTagProfile>>
    {
        public string reviewTagId { get; set; }
    }
    public class GetReviewTagByIdValidator : AbstractValidator<GetReviewTagByIdQueryRequest>
    {
        public GetReviewTagByIdValidator()
        {
            RuleFor(x => x.reviewTagId)
               .Must(x => Guid.TryParse(x, out var result) == true)
               .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
