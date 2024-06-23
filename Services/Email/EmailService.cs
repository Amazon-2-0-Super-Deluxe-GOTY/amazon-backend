using MailKit.Net.Smtp;
using MimeKit;

namespace amazon_backend.Services.Email
{
    public class EmailService : IEmailService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly GmailClient _gmailClient;
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpClient _smtpClient;
        private bool _disposed;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _gmailClient = new();
            _disposed = false;
            try
            {
                configuration.GetSection("Smtp:Gmail").Bind(_gmailClient);
            }
            catch (Exception _)
            {
                throw new Exception("Not found section SMTP:Gmail");
            }
            _smtpClient = new();
            try
            {
                _smtpClient.Connect(_gmailClient.Host, _gmailClient.Port, MailKit.Security.SecureSocketOptions.StartTls);
                _smtpClient.Authenticate(_gmailClient.Email, _gmailClient.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed connect to SMTP: {ex.Message}");
            }
        }

        public async Task<bool> SendEmailAsync(string recipient, string subject, string message)
        {
            using var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Perry Team", _gmailClient.Email));
            mailMessage.To.Add(new MailboxAddress("", recipient));
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };
            try
            {
                await _smtpClient.SendAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email service failed: {ex.Message}");

            }
            return false;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_smtpClient.IsConnected)
                {
                    _smtpClient.Disconnect(true);
                }
                _smtpClient.Dispose();
            }
        }
    }
}

