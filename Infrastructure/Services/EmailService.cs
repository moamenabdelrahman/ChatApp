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
            string mailServer = configuration["Email:MailServer"];
            int port = Convert.ToInt32(configuration["Email:MailPort"]);
            string fromEmail = configuration["Email:FromEmail"];
            string password = configuration["Email:Password"];
            string senderName = configuration["Email:SenderName"];

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
