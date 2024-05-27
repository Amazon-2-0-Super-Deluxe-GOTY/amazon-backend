using amazon_backend.Models;
using amazon_backend.Profiles.JwtTokenProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.UserRequests
{
    public class CreateUserCommandRequest:IRequest<Result<JwtTokenProfile>>
    {
        public string email { get; set; }
        public string password { get; set; }
    }
    public class CreateUserValidator : AbstractValidator<CreateUserCommandRequest>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.email).EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
                .NotEmpty();
            RuleFor(p => p.password).NotEmpty().WithMessage("Your password cannot be empty")
                     .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                     .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                     .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                     .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                     .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
        }
    }
}
