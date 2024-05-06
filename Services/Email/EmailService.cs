using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Hosting;



namespace amazon_backend.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public void SendEmail(string recipient, string subject, string message)
        {
            string fromEmail = "testasp.201project@gmail.com";
            string fromPassword = "fbknrqdoduuinxpa";

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromEmail);
            mailMessage.To.Add(recipient);
            mailMessage.Subject = subject;
            mailMessage.Body = message;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);
            smtpClient.EnableSsl = true;

            smtpClient.Send(mailMessage);
        }


        
    }
}

