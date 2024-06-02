using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.ProductRequests
{
    public class GetProductsQueryRequest : IRequest<Result<List<ProductCardProfile>>>
    {
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public bool? discount { get; set; }
        public int? categoryId { get; set; }
        public string? rating { get; set; }
        public string? price { get; set; }
        public string? orderBy { get; set; }
    }
    public class GetProductsByCategoryValidator : AbstractValidator<GetProductsQueryRequest>
    {
        public GetProductsByCategoryValidator()
        {
            RuleFor(x => x.pageSize).NotEmpty().GreaterThan(0);
            RuleFor(x => x.pageIndex).NotEmpty().GreaterThan(0);
            RuleFor(x => x.price).Must((price) =>
            {
                var parsedPrice = price!.Split('-');
                if (parsedPrice.Length != 2) return false;
                foreach (var item in parsedPrice)
                {
                    if (!int.TryParse(item, out var _)) return false;
                }
                if (int.Parse(parsedPrice[0]) > int.Parse(parsedPrice[1])) return false;
                return true;
            }).When(x => x.price != null).WithMessage("Inccorect format. Use: minPrice-maxPrice");
            RuleFor(x => x.rating).Must((rating) =>
            {
                var parsedRating = rating!.Split(",");
                foreach (var item in parsedRating)
                {
                    if (int.TryParse(item, out var rate))
                    {
                        if (rate < 1 || rate > 5) return false;
                    }
                    else return false;
                }
                return true;
            }).When(x => x.rating != null).WithMessage("Rating must be integer. Rating must be greater than 0 and less than 6");
        }
    }
}
