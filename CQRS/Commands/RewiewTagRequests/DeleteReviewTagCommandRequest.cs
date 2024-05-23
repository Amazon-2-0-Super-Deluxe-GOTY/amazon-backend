using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.RewiewTagRequests
{
    public class DeleteReviewTagCommandRequest:IRequest<Result<Guid>>
    {
        //public string userToken { get; set; }
        public string reviewTagId { get; set; }
        public string? reviewId { get; set; }
    }
    public class DeleteReviewTagValidator : AbstractValidator<DeleteReviewTagCommandRequest>
    {
        public DeleteReviewTagValidator()
        {
            RuleFor(x => x.reviewTagId)
              .Must(x => Guid.TryParse(x, out var result) == true)
              .WithMessage("Incorrect {PropertyName} format");
            RuleFor(x => x.reviewId)
             .Must(x => Guid.TryParse(x, out var result) == true)
             .When(x=>!string.IsNullOrEmpty(x.reviewId))
             .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
