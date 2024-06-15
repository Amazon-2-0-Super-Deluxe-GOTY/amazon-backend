using amazon_backend.Models;
using amazon_backend.Profiles.UserProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.UserRequests
{
    public class RestoreUserCommandRequest : IRequest<Result<ClientProfile>>
    {
        public string userId { get; set; }
    }
    public class RestoreUserValidator : AbstractValidator<RestoreUserCommandRequest>
    {
        public RestoreUserValidator()
        {
            RuleFor(x => x.userId)
               .Must(x => Guid.TryParse(x, out var result) == true)
               .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
