using amazon_backend.Models;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.ReviewsRequests
{
    public class GetReviewsStatsQueryRequest:IRequest<Result<object>>
    {
        public string productId { get; set; }
    }
    public class GetReviewsStatsValidator : AbstractValidator<GetReviewsStatsQueryRequest>
    {
        public GetReviewsStatsValidator()
        {
            RuleFor(x => x.productId).NotEmpty().WithMessage("Product id can not be empty");
            RuleFor(x => x.productId).Must(x => Guid.TryParse(x, out var result) == true).WithMessage("Incorrect {PropertyName} format");
        }
    }
}
