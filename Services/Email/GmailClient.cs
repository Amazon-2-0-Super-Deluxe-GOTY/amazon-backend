namespace amazon_backend.Services.Email
{
    public class GmailClient
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Ssl { get; set; }
    }
}
