using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.RewiewTagRequests
{
    public class CreateReviewTagCommandRequest:IRequest<Result<ReviewTagProfile>>
    {
        public string name { get; set; }
        public string? description { get; set; }
    }
    public class CreateReviewTagValidator : AbstractValidator<CreateReviewTagCommandRequest>
    {
        public CreateReviewTagValidator()
        {
            RuleFor(x => x.name).NotEmpty();
        }
    }
}
