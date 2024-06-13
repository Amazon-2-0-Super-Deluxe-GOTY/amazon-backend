using amazon_backend.CQRS.Commands.ReviewImageRequests;
using amazon_backend.Models;
using amazon_backend.Profiles.CategoryProfiles;
using amazon_backend.Profiles.ReviewImageProfiles;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
namespace amazon_backend.CQRS.Commands.CategoryImageRequst
{
    public class RemoveCategoryImageCommandRequst : IRequest<Result<string>>
    {
        public string categoryImageId { get; set; }
    }
    public class RemoveCategoryImageValidator : AbstractValidator<RemoveCategoryImageCommandRequst>
    {
        public RemoveCategoryImageValidator()
        {
            RuleFor(x => x.categoryImageId)
                .Must(x => Guid.TryParse(x, out var result) == true)
                .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
