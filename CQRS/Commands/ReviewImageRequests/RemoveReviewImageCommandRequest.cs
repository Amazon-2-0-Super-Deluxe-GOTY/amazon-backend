using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.ReviewImageRequests
{
    public class RemoveReviewImageCommandRequest : IRequest<Result<string>>
    {
        public string reviewImageId { get; set; }
    }
    public class RemoveReviewImageValidator : AbstractValidator<RemoveReviewImageCommandRequest>
    {
        public RemoveReviewImageValidator()
        {
            RuleFor(x => x.reviewImageId)
                .Must(x => Guid.TryParse(x, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
