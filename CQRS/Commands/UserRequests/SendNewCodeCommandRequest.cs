using amazon_backend.Data;
using amazon_backend.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Commands.UserRequests
{
    public class SendNewCodeCommandRequest:IRequest<Result<string>>
    {
        public string? email { get; set; }
    }
    public class SendNewCodeValidator : AbstractValidator<SendNewCodeCommandRequest>
    {
        private readonly DataContext _dataContext;

        public SendNewCodeValidator(DataContext dataContext)
        {
            _dataContext = dataContext;
            RuleFor(x => x.email)
               .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
               .When(x => x.email != null);

            RuleFor(x => x.email).MustAsync(async (email, cancellation) =>
            {
                if (email == null) return true;
                var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.DeletedAt == null);
                return user == null;
            }).WithMessage("Email already used").When(x => x.email != null);
        }
    }
}
