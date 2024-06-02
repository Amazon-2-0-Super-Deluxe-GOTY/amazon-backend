namespace amazon_backend.Services.Email
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string recipient, string subject, string message);
    }
}
