using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Hosting;



namespace amazon_backend.Services.Email
{
    public class EmailService : IEmailService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;
        private bool _disposed;
        private readonly string? _fromEmail;
        private readonly string? _fromPassword;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _disposed = false;
            #region create smtp client
            string? host = _configuration["Smtp:Gmail:Host"];
            if (host is null)
                throw new MissingFieldException(":Missing configuration 'Smtp:Gmail:Host'");
            _fromEmail = _configuration["Smtp:Gmail:Email"];
            if (_fromEmail is null)
                throw new MissingFieldException(":Missing configuration 'Smtp:Gmail:Email'");
            _fromPassword = _configuration["Smtp:Gmail:Password"];
            if (_fromPassword is null)
                throw new MissingFieldException(":Missing configuration 'Smtp:Gmail:Password'");
            int port; try
            {
                port = Convert.ToInt32(_configuration["Smtp:Gmail:Port"]);
            }
            catch
            {
                throw new MissingFieldException(":Missing or invalid configuration 'Smtp:Gmail:Port'");
            }
            bool ssl;
            try
            {
                ssl = Convert.ToBoolean(_configuration["Smtp:Gmail:Ssl"]);
            }
            catch
            {
                throw new MissingFieldException(":Missing or invalid configuration 'Smtp:Gmail:Ssl'");
            }
            _smtpClient = new(host, port)
            {
                EnableSsl = ssl,
                Credentials = new NetworkCredential(_fromEmail, _fromPassword)
            };
            _logger = logger;
            #endregion
        }
        public async Task<bool> SendEmailAsync(string recipient, string subject, string message)
        {
            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(_fromEmail!),
                Subject = subject,
                Body = message
            };
            mailMessage.To.Add(recipient);
            try
            {
                await _smtpClient.SendMailAsync(mailMessage);
            }
            catch(SmtpException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
            return true;
        }
        public void Dispose()
        {
            if (!_disposed)
            {
                _smtpClient.Dispose();
                _disposed = true;
            }
        }
    }
}

