using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.RewiewTagRequests
{
    public class DeleteReviewTagFromReviewCommandRequest:IRequest<Result<string>>
    {
        public string reviewId { get; set; }
        public string reviewTagId { get; set; }
    }
    public class DeleteReviewTagFromReviewValidator : AbstractValidator<DeleteReviewTagFromReviewCommandRequest>
    {
        public DeleteReviewTagFromReviewValidator()
        {
            RuleFor(x => x.reviewTagId)
              .Must(x => Guid.TryParse(x, out var result) == true)
              .WithMessage("Incorrect {PropertyName} format");
            RuleFor(x => x.reviewId)
             .Must(x => Guid.TryParse(x, out var result) == true)
             .When(x => !string.IsNullOrEmpty(x.reviewId))
             .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
