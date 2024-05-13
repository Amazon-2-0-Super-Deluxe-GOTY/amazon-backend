using amazon_backend.Models;
using amazon_backend.Profiles.CategoryProfiles;

namespace amazon_backend.Profiles.ProductProfiles
{
    public class ProductViewProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public CategoryViewProfile Category { get; set; }
        public string? ShortDescription { get; set; }
        public double Price { get; set; }
        public string? ImageUrl { get; set; }
        public double? DiscountPrice { get; set; }
        public string Brand { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductPropProfile>? pProps { get; set; }
        public List<ProductImageProfile>? productImages { get; set; }
        public List<AboutProductProfile>? AboutProductItems { get; set; }
        public List<RatingStat>? RatingStats { get; set; }
    }
}
