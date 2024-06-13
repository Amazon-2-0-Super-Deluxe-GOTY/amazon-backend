using amazon_backend.Profiles.CartItemProfiles;

namespace amazon_backend.Profiles.CartProfiles
{
    public class CartProfile
    {
        public Guid Id { get; set; }
        public double TotalPrice { get; set; }
        public List<CartItemProfile>? CartItems { get; set; }
    }
}
