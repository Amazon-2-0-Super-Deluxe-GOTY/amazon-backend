using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.ProductCommands
{
    public class DeleteProductCommandRequest : IRequest<Result<string>>
    {
        public List<string> productIds { get; set; }
    }
    public class DeleteProductValidator : AbstractValidator<DeleteProductCommandRequest>
    {
        public DeleteProductValidator()
        {
            RuleFor(x => x.productIds)
                .Must(productIds => productIds.Count >= 1)
                .WithMessage("Add at least one product Id");

            RuleForEach(x => x.productIds).ChildRules(productIds =>
            {
                productIds.RuleFor(x => x)
                .Must(id => Guid.TryParse(id, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format"); ;
            });
        }
    }
}
