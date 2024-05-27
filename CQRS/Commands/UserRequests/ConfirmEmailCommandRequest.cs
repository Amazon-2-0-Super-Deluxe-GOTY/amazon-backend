using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.UserRequests
{
    public class ConfirmEmailCommandRequest:IRequest<Result<string>>
    {
        public string emailCode { get; set; }
    }
    public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommandRequest>
    {
        public ConfirmEmailValidator()
        {
            RuleFor(x => x.emailCode)
                .MaximumLength(6)
                .WithMessage("The length of the confirmation code must be 6 characters")
                .NotEmpty()
                .WithMessage("Email code required");
        }
    }
}
