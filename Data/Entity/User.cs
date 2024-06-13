namespace amazon_backend.Data.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? TempEmail { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }
        public string? EmailCode { get; set; }
        
        //Navigation props
        public List<Review>? Reviews { get; set; }
        public List<ReviewImage>? ReviewImages { get; set; }
        public List<TokenJournal>? TokenJournals { get; set; }
        public List<ReviewLike>? ReviewLikes { get; set; }
        public List<Product>? WishedProducts { get; set; }
        public List<Cart>? Carts { get; set; }
        public List<Order>? Orders { get; set; }
    }
}
