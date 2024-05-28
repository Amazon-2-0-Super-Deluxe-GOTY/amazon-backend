namespace amazon_backend.Services.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipient, string subject, string message);
    }
}
