namespace amazon_backend.Services.Email
{
    public interface IEmailService
    {
       
        void SendEmail(string recipient, string subject, string message);
    }
}
