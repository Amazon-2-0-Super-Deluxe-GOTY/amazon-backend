using amazon_backend.Models;
using amazon_backend.Profiles.JwtTokenProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.UserRequests
{
    public class LoginUserCommandRequest:IRequest<Result<JwtTokenProfile>>
    {
        public string email { get; set; }
        public string password { get; set; }
    }
    public class LoginUserValidator : AbstractValidator<LoginUserCommandRequest>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.email)
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
                .WithMessage("The email does not match the pattern: example@service.com")
                .NotEmpty()
                .WithMessage("Email required");
            RuleFor(x => x.password)
                .NotEmpty()
                .WithMessage("Password required");
        }
    }
}
