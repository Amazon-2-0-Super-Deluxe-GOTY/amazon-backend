using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.CartRequests
{
    public class RemoveItemsFromCartCommandRequest : IRequest<Result<string>>
    {
        public List<string> cartItemIds { get; set; }
    }
    public class RemoveItemsFromCartValidator : AbstractValidator<RemoveItemsFromCartCommandRequest>
    {
        public RemoveItemsFromCartValidator()
        {
            RuleFor(x => x.cartItemIds)
               .Must(cartItemIds => cartItemIds.Count >= 1)
               .WithMessage("Add at least one cart item Id");

            RuleForEach(x => x.cartItemIds).ChildRules(cartItemIds =>
            {
                cartItemIds.RuleFor(x => x)
                .Must(id => Guid.TryParse(id, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format");
            });
        }
    }
}
