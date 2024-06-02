using amazon_backend.Data.Entity;
using amazon_backend.Profiles.CategoryProfiles;

namespace amazon_backend.Profiles.ProductProfiles
{
    public class ProductViewProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public CategoryProductProfile Category { get; set; }
        public string? Code { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public int? DiscountPercent { get; set; }
        public double? DiscountPrice { get; set; }
        public string? ImageUrl { get; set; }
        public double? GeneralRate { get; set; }
        public int? ReviewsQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductImageProfile>? ProductImages { get; set; }
        public List<ProductPropProfile>? ProductProperties { get; set; }
        public List<AboutProductProfile>? AboutProductItems { get; set; }
    }
}
