namespace amazon_backend.Data.Entity
{
    public class TokenJournal
    {
        public Guid Id { get; set; }
        public Guid TokenId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime ActivatedAt { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public JwtToken? Token { get; set; }
        public User? User { get; set; }
    }
}
