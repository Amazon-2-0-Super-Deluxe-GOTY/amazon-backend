using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.ReviewRequests
{
    public class DeleteReviewCommandRequest:IRequest<Result<Guid>>
    {
        //public string userToken { get; set; }
        public string reviewId { get; set; }
    }
    public class DeleteReviewCommandValidator:AbstractValidator<DeleteReviewCommandRequest>
    {
        public DeleteReviewCommandValidator()
        {
            RuleFor(x => x.reviewId)
                 .Must(x => Guid.TryParse(x, out var result) == true)
                 .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
