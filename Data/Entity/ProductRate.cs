namespace amazon_backend.Data.Entity
{
    public class ProductRate
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Mark { get; set; }

        public Product? Product { get; set; }
        public User? User { get; set; }
    }
}
