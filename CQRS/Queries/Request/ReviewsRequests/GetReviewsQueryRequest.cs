using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.ReviewsRequests
{
    public class GetReviewsQueryRequest:IRequest<Result<List<ReviewProfile>>>
    {
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public string? productId { get; set; }
        public string? userId { get; set; }
        public int? rating { get; set; }
        public bool? byAsc { get; set; }
    }
    public class GetReviewQueryValidator:AbstractValidator<GetReviewsQueryRequest>
    {
        public GetReviewQueryValidator()
        {
            RuleFor(x => x.pageSize).NotEmpty().GreaterThan(0);
            RuleFor(x => x.pageIndex).NotEmpty().GreaterThan(0);
            RuleFor(x=>x.productId)
                .Must(x => Guid.TryParse(x, out var result) == true)
                .When(x=>string.IsNullOrEmpty(x.productId)==false)
                .WithMessage("Incorrect {PropertyName} format");
            RuleFor(x => x.rating).GreaterThan(0).LessThan(6);
            RuleFor(x => x.userId)
                 .Must(x => Guid.TryParse(x, out var result) == true)
                 .When(x => string.IsNullOrEmpty(x.userId) == false)
                 .WithMessage("Incorrect {PropertyName} format");
        }
    }
}
