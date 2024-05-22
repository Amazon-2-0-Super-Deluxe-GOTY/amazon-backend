using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.ReviewRequests
{
    public class DeleteReviewImageCommandRequest:IRequest<Result<Guid>>
    {
        public string imageId { get; set; }
    }
    public class DeleteReviewImageValidator : AbstractValidator<DeleteReviewImageCommandRequest>
    {
        public DeleteReviewImageValidator()
        {
            RuleFor(x => x.imageId)
              .Must(x => Guid.TryParse(x, out var result) == true)
              .WithMessage("Incorrect {PropertyName} format"); ;
        }
    }
}
