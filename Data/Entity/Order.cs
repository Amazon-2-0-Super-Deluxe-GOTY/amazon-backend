
namespace amazon_backend.Data.Entity
{
    public class Order
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }  
        public Guid UserId { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // navigation props
        public User? User { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
        public List<DeliveryAddress>? DeliveryAddresses { get; set; }
    }
}