namespace amazon_backend.Profiles.OrderItemProfiles
{
    public class OrderItemProfile
    {
        public string Name { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
    }
}
