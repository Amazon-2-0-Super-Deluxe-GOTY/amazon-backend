using amazon_backend.CQRS.Commands.ReviewRequests;
using amazon_backend.Data.Entity;
using amazon_backend.Data.Model;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using FluentValidation;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace amazon_backend.CQRS.Commands.CategoryPropertyKeyRequests
{
    public class UpdateCategoryPropertyKeyCommandRequst : IRequest<Result<CategoryPropertyKeyProfile>>
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public bool IsFilter { get; set; }
        public bool IsRequired { get; set; }
        [Required]
        public string NameCategory { get; set; }
        public bool IsDeleted { get; set; }
        public uint CategoryId { get; set; }
    }
    public class UpdateCategoryPropertyKeyCommandValidator : AbstractValidator<UpdateCategoryPropertyKeyCommandRequst>
    {
        public UpdateCategoryPropertyKeyCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.IsFilter).NotEmpty();
            RuleFor(x => x.NameCategory).NotEmpty();
        }
    }
}
