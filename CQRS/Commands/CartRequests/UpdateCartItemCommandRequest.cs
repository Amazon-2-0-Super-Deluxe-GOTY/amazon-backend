using amazon_backend.Models;
using amazon_backend.Profiles.CartItemProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.CartRequests
{
    public class UpdateCartItemCommandRequest : IRequest<Result<CartItemProfile>>
    {
        public string cartItemId { get; set; }
        public int quantity { get; set; }
    }
    public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemCommandRequest>
    {
        public UpdateCartItemValidator()
        {
            RuleFor(x => x.cartItemId)
                .Must(id => Guid.TryParse(id, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format");
            RuleFor(x => x.quantity).GreaterThan(0).WithMessage("Quantity must be a positive value");
        }
    }
}
