namespace amazon_backend.Data.Entity
{
    public class JwtToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
        public List<TokenJournal>? TokenJournals { get; set; }
    }
}
