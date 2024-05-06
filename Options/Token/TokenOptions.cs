namespace amazon_backend.Options.Token
{
    public class TokenOptions
    {
        public string SecretKey { get; set; }
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(SecretKey))
                throw new ArgumentNullException(nameof(SecretKey),
                    "The secret key cannot be null or empty. Please provide a valid secret in the TokenOptions.");
        }
    }
}
