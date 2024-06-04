using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.ProductCommands
{
    public class DeleteProductCommandRequest : IRequest<Result<string>>
    {
        public string productId { get; set; }
    }
    public class DeleteProductValidator : AbstractValidator<DeleteProductCommandRequest>
    {
        public DeleteProductValidator()
        {
            RuleFor(x => x.productId)
                .Must(x => Guid.TryParse(x, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
