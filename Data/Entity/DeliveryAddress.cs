namespace amazon_backend.Data.Entity
{
    public class DeliveryAddress
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PostIndex { get; set; }

        // navigation props
        public Order? Order { get; set; }
    }
}
