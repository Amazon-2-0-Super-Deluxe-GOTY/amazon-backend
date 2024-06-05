using amazon_backend.Models;
using FluentValidation;
using MediatR;
namespace amazon_backend.CQRS.Commands.CategoryPropertyKeyRequests
{
    public class DeleteCategoryPropertyKeyCommandRequst : IRequest<Result<Guid>>
    {
        public string CategoryPropertyKeyName {  get; set; }

    }
    public class DeleteCategoryPropertyKeyCommandValidator : AbstractValidator<DeleteCategoryPropertyKeyCommandRequst>
    {
        public DeleteCategoryPropertyKeyCommandValidator()
        {
            RuleFor(x => x.CategoryPropertyKeyName).NotEmpty();

        }
    }
}
