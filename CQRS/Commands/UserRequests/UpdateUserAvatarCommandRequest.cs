using amazon_backend.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace amazon_backend.CQRS.Commands.UserRequests
{
    public class UpdateUserAvatarCommandRequest : IRequest<Result<string>>
    {
        public IFormFile? avatar { get; set; }
    }
    public class UpdateUserAvatarValidator : AbstractValidator<UpdateUserAvatarCommandRequest>
    {
        public UpdateUserAvatarValidator()
        {
            RuleFor(x => x.avatar!.Length).LessThanOrEqualTo(5 * 1024 * 1024)
                    .WithMessage("File size must be less than or equal to 5 MB")
                    .When(x => x.avatar != null);
            RuleFor(x => x.avatar!.ContentType).Must(contentType =>
                contentType == "image/jpeg" || contentType == "image/png")
                .WithMessage("Only JPEG and PNG files are allowed")
                .When(x => x.avatar != null);
        }
    }
}
