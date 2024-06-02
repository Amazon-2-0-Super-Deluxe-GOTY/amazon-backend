namespace amazon_backend.Data.Entity
{
    public class ReviewLike
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ReviewId { get; set; }
        public User? User { get; set; }
        public Review? Review { get; set; }
    }
}
