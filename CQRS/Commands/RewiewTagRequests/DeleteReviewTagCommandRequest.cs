using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.RewiewTagRequests
{
    public class DeleteReviewTagCommandRequest:IRequest<Result<string>>
    {
        public string reviewTagId { get; set; }
    }
    public class DeleteReviewTagValidator : AbstractValidator<DeleteReviewTagCommandRequest>
    {
        public DeleteReviewTagValidator()
        {
            RuleFor(x => x.reviewTagId)
              .Must(x => Guid.TryParse(x, out var result) == true)
              .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
