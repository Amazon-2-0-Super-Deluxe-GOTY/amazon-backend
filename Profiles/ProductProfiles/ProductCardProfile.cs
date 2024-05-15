namespace amazon_backend.Profiles.ProductProfiles
{
    public class ProductCardProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double? DiscountPrice { get; set; }
        public double Price { get; set; }
        public int? DiscountPercentage { get; set; }
        public double GeneralRate { get; set; }
        public int ReviewsCount { get; set; }
        public string? ImageUrl { get; set; }
    }
}
