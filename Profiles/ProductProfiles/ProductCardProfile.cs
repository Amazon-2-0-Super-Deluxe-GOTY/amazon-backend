namespace amazon_backend.Profiles.ProductProfiles
{
    public class ProductCardProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double? DiscountPrice { get; set; }
        public double Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
