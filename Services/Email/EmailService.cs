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




        public bool Send(string mailTemplate, object model)//njnj
        {
            String? template = null;
        //    String[] filenames = new String[]
        //{
        //        mailTemplate,
        //        mailTemplate+".html",
        //        "Services/Email/"+mailTemplate,
        //        "Services/Email/"+mailTemplate+".html"
        //};
            string relativePath = Path.Combine("wwwroot", "data", "index.html");

            // Get the absolute path to the file based on the current working directory
            string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            // Check if the file exists
            bool fileExists = File.Exists(absolutePath);

            if (fileExists)
            {
                template = System.IO.File.ReadAllText(absolutePath);
            }
            else
            {
                // The file does not exist
                throw new ArgumentException($"Template'{fileExists}' not Exists" + $"{absolutePath}");
            }
            







            //foreach (String filename in filenames)
            //{
            //    if (System.IO.File.Exists(filename))
            //    {
            //        template = System.IO.File.ReadAllText(filename);

            //        //throw new Exception($"{template}, {filename}");
            //        break;
            //    }
            //}


            if (template is null)
            {
                throw new ArgumentException($"Template'{mailTemplate}' not found" + $"{template}");
                                                                                                                
            }

            // Перевіряємо поштову конфігурацію
            String? host = _configuration["Smtp:Gmail:Host"];
            if (host is null)
                throw new MissingFieldException(":Missing configuration 'Smtp:Gmail:Host'");
            String? mailbox = _configuration["Smtp:Gmail:Email"];
            if (mailbox is null)
                throw new MissingFieldException(":Missing configuration 'Smtp:Gmail:Email'");
            String? password = _configuration["Smtp:Gmail:Password"];
            if (password is null)
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

            // Заповнюємо шаблон - проходимо по властивості моделі
            // та замінюємо їх значення у шаблоні за збігом імен
            String? userEmail = null;
            foreach (var prop in model.GetType().GetProperties())
            {
                if (prop.Name == "Email") userEmail = prop.GetValue(model)?.ToString();
                String placeholder = $"{{{{{prop.Name}}}}}";
                if (template.Contains(placeholder))
                {
                    template = template
                        .Replace(placeholder, prop.GetValue(model)?.ToString() ?? "");
                }
            }
            if (userEmail is null)
            {
                throw new ArgumentException("No 'Email' property in model");
            }
            // TODO: перевірити залишок {{\w+}} плейсхолдерів у шаблоні

            using SmtpClient smtpClient = new(host, port)
            {
                //EnableSsl = ssl,
                Credentials = new NetworkCredential(mailbox, password)
            };
            MailMessage mailMessage = new()
            {
                From = new MailAddress(mailbox),
                Subject = "Інформація про Ваш новий профіль ASTIN",
                IsBodyHtml = true,
                Body = template
            };
            mailMessage.To.Add(userEmail);
            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Send Email exception {ex.Message}");
                return false;
            }
        }
    }
}

