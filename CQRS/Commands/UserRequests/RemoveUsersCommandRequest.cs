using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.UserRequests
{
    public class RemoveUsersCommandRequest : IRequest<Result<string>>
    {
        public List<string> usersIds { get; set; }
    }
    public class RemoveUsersValidator : AbstractValidator<RemoveUsersCommandRequest>
    {
        public RemoveUsersValidator()
        {
            RuleFor(x => x.usersIds)
             .Must(usersIds => usersIds.Count >= 1)
             .WithMessage("Add at least one product Id");

            RuleForEach(x => x.usersIds).ChildRules(usersIds =>
            {
                usersIds.RuleFor(x => x)
                .Must(id => Guid.TryParse(id, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format"); ;
            });
        }
    }
}
