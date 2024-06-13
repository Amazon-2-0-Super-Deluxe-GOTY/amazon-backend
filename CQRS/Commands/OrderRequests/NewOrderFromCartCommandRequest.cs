using amazon_backend.Models;
using amazon_backend.Profiles.OrderProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Commands.OrderRequests
{
    public class NewOrderFromCartCommandRequest : IRequest<Result<OrderProfile>>
    {
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string postIndex { get; set; }
        public string paymentMethod { get; set; }
    }
    public class NewOrderFromCartValidator : AbstractValidator<NewOrderFromCartCommandRequest>
    {
        public NewOrderFromCartValidator()
        {
            RuleFor(x => x.country).NotEmpty().WithMessage("Country cannot be empty");
            RuleFor(x => x.state).NotEmpty().WithMessage("State cannot be empty");
            RuleFor(x => x.city).NotEmpty().WithMessage("City cannot be empty");
            RuleFor(x => x.postIndex)
                .Must((PostIndex) =>
                {
                    foreach (var item in PostIndex)
                    {
                        if (!int.TryParse(item.ToString(), out var _))
                        { 
                            return false;
                        }
                    }
                    return true;
                }).WithMessage("Post index must contain a numbers")
                .NotEmpty().WithMessage("Post index cannot be empty")
                .MaximumLength(9).WithMessage("Length of index must be maximum 9");
            RuleFor(x => x.paymentMethod).NotEmpty().WithMessage("Payment method cannot be empty")
                .Must((method) =>
                {
                    if (method.ToLower() == "cash" || method.ToLower() == "card") return true;
                    return false;
                }).WithMessage("Payment method can be only Cash or Card");
        }
    }
}
