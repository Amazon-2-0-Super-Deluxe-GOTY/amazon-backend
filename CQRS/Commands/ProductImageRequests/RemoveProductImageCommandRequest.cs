using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.ReviewImageRequests
{
    public class RemoveProductImageCommandRequest : IRequest<Result<string>>
    {
        public string productImageId { get; set; }
    }
    public class RemoveProductImageValidator : AbstractValidator<RemoveProductImageCommandRequest>
    {
        public RemoveProductImageValidator()
        {
            RuleFor(x => x.productImageId)
                .Must(x => Guid.TryParse(x, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
