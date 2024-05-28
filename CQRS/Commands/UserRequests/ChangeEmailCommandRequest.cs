using amazon_backend.Data;
using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.UserRequests
{
    public class ChangeEmailCommandRequest:IRequest<Result<string>>
    {
        public string newEmail { get; set; }
    }
    public class ChangeEmailValidator : AbstractValidator<ChangeEmailCommandRequest>
    {
        private readonly DataContext _dataContext;

        public ChangeEmailValidator(DataContext dataContext)
        {
            _dataContext = dataContext;

            RuleFor(x => x.newEmail)
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
                .NotEmpty();

            RuleFor(x => x.newEmail).MustAsync(async (email, cancellation) =>
            {
                var user = _dataContext.Users.FirstOrDefault(u => u.Email == email && u.DeletedAt == null);
                return user == null;
            }).WithMessage("Email already used");
        }
    }
}
