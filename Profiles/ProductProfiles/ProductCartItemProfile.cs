namespace amazon_backend.Profiles.ProductProfiles
{
    public class ProductCartItemProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public double? DiscountPrice { get; set; }
        public double Price { get; set; }
        public int? DiscountPercent { get; set; }
    }
}
