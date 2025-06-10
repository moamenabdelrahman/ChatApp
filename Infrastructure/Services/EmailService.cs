using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services
{
    public class EmailService
    {
        private SmtpClient Client { get; set; }

        private MailAddress FromEmail { get; set; }

        public EmailService(IConfiguration configuration)
        {
            string mailServer = configuration.GetSection("Email")["MailServer"];
            int port = Convert.ToInt32(configuration.GetSection("Email")["MailPort"]);
            string fromEmail = configuration.GetSection("Email")["FromEmail"];
            string password = configuration.GetSection("Email")["Password"];
            string senderName = configuration.GetSection("Email")["SenderName"];

            Client = new SmtpClient(mailServer, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            FromEmail = new MailAddress(fromEmail, senderName);
        }

        public Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MailMessage()
            {
                From = FromEmail,
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            message.To.Add(toEmail);

            return Client.SendMailAsync(message);
        }
    }
}
