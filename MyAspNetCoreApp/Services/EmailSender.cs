using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MyAspNetCoreApp.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? throw new InvalidOperationException("SMTP Server configuration is missing");
            var smtpPortStr = _configuration["EmailSettings:SmtpPort"] ?? throw new InvalidOperationException("SMTP Port configuration is missing");
            var smtpUser = _configuration["EmailSettings:SmtpUsername"] ?? throw new InvalidOperationException("SMTP Username configuration is missing");
            var smtpPass = _configuration["EmailSettings:SmtpPassword"] ?? throw new InvalidOperationException("SMTP Password configuration is missing");
            var enableSslStr = _configuration["EmailSettings:EnableSsl"] ?? "true";

            var smtpPort = int.Parse(smtpPortStr);
            var enableSsl = bool.Parse(enableSslStr);

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = enableSsl;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
