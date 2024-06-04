using amazon_backend.Profiles.ProductImageProfiles;

namespace amazon_backend.Profiles.ProductProfiles
{
    public class ProductCardProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public double? DiscountPrice { get; set; }
        public double Price { get; set; }
        public int? DiscountPercent { get; set; }
        public double GeneralRate { get; set; }
        public int ReviewsCount { get; set; }
        public List<ProductImageCardProfile> ProductImages { get; set; }
    }
}
