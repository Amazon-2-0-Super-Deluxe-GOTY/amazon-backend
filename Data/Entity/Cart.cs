namespace amazon_backend.Data.Entity
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public User? User { get; set; }
        public List<CartItem>? CartItems { get; set; }
    }
}
