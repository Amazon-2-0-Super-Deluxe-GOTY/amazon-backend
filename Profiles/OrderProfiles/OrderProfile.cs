using amazon_backend.Profiles.DeliveryAddressProfiles;
using amazon_backend.Profiles.OrderItemProfiles;

namespace amazon_backend.Profiles.OrderProfiles
{
    public class OrderProfile
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<OrderItemProfile>? OrderItems { get; set; }
        public List<DeliveryAddressProfile>? DeliveryAddresses { get; set; }
    }
}
