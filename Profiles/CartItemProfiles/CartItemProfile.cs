using amazon_backend.Profiles.ProductProfiles;

namespace amazon_backend.Profiles.CartItemProfiles
{
    public class CartItemProfile
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public ProductCartItemProfile Product { get; set; }
    }
}
