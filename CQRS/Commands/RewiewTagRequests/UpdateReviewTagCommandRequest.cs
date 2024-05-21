using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.RewiewTagRequests
{
    public class UpdateReviewTagCommandRequest:IRequest<Result<Guid>>
    {
        public string reviewTagId { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
    }
    public class UpdateReviewTagValidator : AbstractValidator<UpdateReviewTagCommandRequest>
    {
        public UpdateReviewTagValidator()
        {
            RuleFor(x => x.name).NotEmpty();
            RuleFor(x => x.reviewTagId)
              .Must(x => Guid.TryParse(x, out var result) == true)
              .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
