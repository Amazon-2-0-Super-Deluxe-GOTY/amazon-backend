using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.UserRequests
{
    public class UpdateUserCommandRequest : IRequest<Result<string>>
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? birthDate { get; set; }
        public string? oldPassword { get; set; }
        public string? newPassword { get; set; }
    }
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommandRequest>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.firstName)
                .NotEmpty()
                .When(x => x.firstName != null);
            RuleFor(x => x.lastName)
                .NotEmpty()
                .When(x => x.lastName != null);
            RuleFor(x => x.birthDate)
                .NotEmpty()
                .Must(x => DateTime.TryParse(x, out var _) == true)
                .WithMessage("Date parsing error")
                .When(x => x.birthDate != null);
            RuleFor(x => x.oldPassword)
                .NotEmpty()
                .WithMessage("To change the password you need to specify the old and new password")
                .When(x => x.newPassword != null);
            RuleFor(x => x.newPassword)
                .NotEmpty()
                .WithMessage("To change the password you need to specify the old and new password")
                .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                .When(x => x.oldPassword != null);
        }
    }
}
